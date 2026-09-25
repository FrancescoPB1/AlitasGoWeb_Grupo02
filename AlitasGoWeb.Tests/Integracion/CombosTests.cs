using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Combos que integran otros productos y descuentan su stock.
    public class CombosTests
    {
        // Productos y datos de la semilla
        private const int MediaDocena = 1, Docena = 2, PapasFritas = 6, GaseosaPersonal = 9, ComboDuo = 13;
        private const int Buffalo = 1, Bbq = 2;
        private const int AlitasCrudas = 1, SalsaBuffalo = 2, SalsaBbq = 3, Gaseosa = 4, Papas = 6;

        private static ComboService Combos(Escenario e) => new(new ComboRepository(e.Db), e.Catalogo, e.Auditoria, e.Uow);
        private static AltaProductoService Alta(Escenario e) => new(e.Catalogo, e.Inventario, Combos(e), e.Uow);

        private static Producto NuevoCombo(string nombre) => new()
        {
            Nombre = nombre,
            IdCategoriaProducto = 1,
            IdTipoProducto = Producto.IdTipoCombo,
            Precio = 45m,
            RequiereSabor = true    // se ignora: un combo nunca pide sabor al tomar el pedido
        };

        private static LineaComponente Comp(int producto, int cantidad, int? sabor = null) =>
            new() { IdProducto = producto, Cantidad = cantidad, IdSabor = sabor };

        [Fact]
        public async Task CrearCombo_GuardaSusProductosYAlVenderDescuentaLaRecetaDeCadaUno()
        {
            using var e = new Escenario();
            var combo = NuevoCombo("Combo Familiar");

            var r = await Alta(e).CrearComboAsync(combo, new List<LineaComponente>
            {
                Comp(Docena, 2, Buffalo),      // 2 docenas buffalo
                Comp(PapasFritas, 1),
                Comp(GaseosaPersonal, 3),
                new()                          // fila vacía del formulario: se ignora
            }, Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            var componentes = await Combos(e).ListarComponentesAsync(combo.IdProducto);
            Assert.Equal(3, componentes.Count);
            using (var ctx = e.NuevoContexto())
                Assert.False(ctx.Productos.Single(p => p.IdProducto == combo.IdProducto).RequiereSabor);

            // Vender 1 combo: 2 docenas (1.20 kg c/u + 0.10 lt buffalo c/u) + 1 papas (0.25 kg) + 3 gaseosas.
            await e.RegistrarAsync(Salon(8, Linea(combo.IdProducto, 1)));
            Assert.Equal(17.60m, e.Stock(AlitasCrudas));
            Assert.Equal(4.80m, e.Stock(SalsaBuffalo));
            Assert.Equal(4m, e.Stock(SalsaBbq));          // no se pidió BBQ
            Assert.Equal(14.75m, e.Stock(Papas));
            Assert.Equal(47m, e.Stock(Gaseosa));
        }

        [Fact]
        public async Task CrearCombo_ConErrores_ExplicaCadaUnoYNoGuardaNada()
        {
            using var e = new Escenario();

            var r = await Alta(e).CrearComboAsync(NuevoCombo("Combo mal armado"), new List<LineaComponente>
            {
                Comp(Docena, 1),                    // lleva sabor y no se eligió
                Comp(PapasFritas, 1, Bbq),          // no lleva sabor
                Comp(ComboDuo, 1),                  // un combo dentro de otro
                Comp(GaseosaPersonal, 0),           // cantidad cero
                Comp(PapasFritas, 2)                // repetido
            }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.StartsWith("Producto 1") && x.Contains("elige cuál"));
            Assert.Contains(r.Errores, x => x.StartsWith("Producto 2") && x.Contains("no lleva sabor"));
            Assert.Contains(r.Errores, x => x.StartsWith("Producto 3") && x.Contains("otro combo"));
            Assert.Contains(r.Errores, x => x.StartsWith("Producto 4") && x.Contains("cantidad"));
            Assert.Contains(r.Errores, x => x.Contains("Papas Fritas") && x.Contains("repetido"));
            using var ctx = e.NuevoContexto();
            Assert.Equal(0, ctx.Productos.Count(p => p.Nombre == "Combo mal armado"));
        }

        [Fact]
        public async Task CrearCombo_SinProductos_NoSeCrea()
        {
            using var e = new Escenario();

            var r = await Alta(e).CrearComboAsync(NuevoCombo("Combo vacío"), new List<LineaComponente> { new() }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("al menos un producto"));
            using var ctx = e.NuevoContexto();
            Assert.Equal(0, ctx.Productos.Count(p => p.Nombre == "Combo vacío"));
        }

        [Fact]
        public async Task EditarCombo_AgregarYQuitarProductos_QuedaEnLaBitacora()
        {
            using var e = new Escenario();
            var combos = Combos(e);

            var agregar = await combos.AgregarComponenteAsync(ComboDuo, Comp(PapasFritas, 2), Admin);
            Assert.True(agregar.Exito, string.Join(" | ", agregar.Errores));
            Assert.False((await combos.AgregarComponenteAsync(ComboDuo, Comp(PapasFritas, 1), Admin)).Exito);   // ya está
            Assert.False((await combos.AgregarComponenteAsync(PapasFritas, Comp(GaseosaPersonal, 1), Admin)).Exito); // no es combo

            var quitar = await combos.QuitarComponenteAsync(ComboDuo, GaseosaPersonal, Admin);
            Assert.True(quitar.Exito, string.Join(" | ", quitar.Errores));

            var actuales = await combos.ListarComponentesAsync(ComboDuo);
            Assert.Equal(new[] { Docena, PapasFritas }, actuales.Select(c => c.IdProductoComponente).OrderBy(x => x));
            using var ctx = e.NuevoContexto();
            Assert.Equal(2, ctx.Auditorias.Count(a => a.Accion == "Modificar combo" && a.IdEntidad == ComboDuo.ToString()));
        }

        [Fact]
        public async Task QuitarElUltimoProducto_NoSePermite()
        {
            using var e = new Escenario();
            var combos = Combos(e);
            Assert.True((await combos.QuitarComponenteAsync(ComboDuo, GaseosaPersonal, Admin)).Exito);

            var r = await combos.QuitarComponenteAsync(ComboDuo, Docena, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("al menos un producto"));
            Assert.Single(await combos.ListarComponentesAsync(ComboDuo));
        }
    }
}
