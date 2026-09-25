using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Crear un producto junto con sus ingredientes (receta), en una sola transacción.
    public class AltaProductoTests
    {
        private const int Guarnicion = 3;      // TipoProducto
        private const int Guarniciones = 3;    // CategoriaProducto
        private const int Papas = 6, SalsaBbq = 3, Gaseosa = 4;   // Insumos

        private static AltaProductoService Alta(Escenario e) =>
            new(e.Catalogo, e.Inventario, new ComboService(new ComboRepository(e.Db), e.Catalogo, e.Auditoria, e.Uow), e.Uow);

        private static Producto Nuevo(string nombre, int tipo = Guarnicion, bool sabor = false) => new()
        {
            Nombre = nombre,
            IdCategoriaProducto = Guarniciones,
            IdTipoProducto = tipo,
            Precio = 12m,
            RequiereSabor = sabor
        };

        private static LineaIngrediente Ing(int insumo, decimal cantidad, int? sabor = null) =>
            new() { IdInsumo = insumo, Cantidad = cantidad, IdSabor = sabor };

        private static int ContarProductos(Escenario e, string nombre)
        {
            using var ctx = e.NuevoContexto();
            return ctx.Productos.Count(p => p.Nombre == nombre);
        }

        [Fact]
        public async Task Crear_GuardaElProductoConSuRecetaYAlVenderDescuentaElStock()
        {
            using var e = new Escenario();
            var producto = Nuevo("Papas con BBQ");

            var r = await Alta(e).CrearConRecetaAsync(producto,
                new List<LineaIngrediente> { Ing(Papas, 0.30m), Ing(SalsaBbq, 0.02m), new() /* fila vacía: se ignora */ }, Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            using (var ctx = e.NuevoContexto())
            {
                var receta = ctx.Recetas.Where(x => x.IdProducto == producto.IdProducto).OrderBy(x => x.IdInsumo).ToList();
                Assert.Equal(new[] { SalsaBbq, Papas }, receta.Select(x => x.IdInsumo));
                Assert.Equal("kg", receta.Single(x => x.IdInsumo == Papas).UnidadMedida);
            }

            // La receta nueva funciona de verdad: vender 2 descuenta 0.60 kg de papas y 0.04 lt de BBQ.
            await e.RegistrarAsync(Salon(5, Linea(producto.IdProducto, 2)));
            Assert.Equal(14.40m, e.Stock(Papas));
            Assert.Equal(3.96m, e.Stock(SalsaBbq));
        }

        [Fact]
        public async Task Crear_SinIngredientes_NoGuardaNada()
        {
            using var e = new Escenario();

            var r = await Alta(e).CrearConRecetaAsync(Nuevo("Producto vacío"), new List<LineaIngrediente> { new() }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("al menos un ingrediente"));
            Assert.Equal(0, ContarProductos(e, "Producto vacío"));
        }

        [Fact]
        public async Task Crear_ConIngredientesInvalidos_ExplicaCadaErrorYNoGuardaNada()
        {
            using var e = new Escenario();

            var r = await Alta(e).CrearConRecetaAsync(Nuevo("Papas mal cargadas"), new List<LineaIngrediente>
            {
                Ing(Papas, 0.30m),
                Ing(999, 1m),               // insumo que no existe
                Ing(SalsaBbq, 0m),          // cantidad cero
                Ing(Papas, 0.10m),          // repetido
                Ing(Gaseosa, 1m, sabor: 2)  // el producto no lleva sabor
            }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.StartsWith("Ingrediente 2") && x.Contains("insumo"));
            Assert.Contains(r.Errores, x => x.StartsWith("Ingrediente 3") && x.Contains("cantidad"));
            Assert.Contains(r.Errores, x => x.StartsWith("Ingrediente 5") && x.Contains("no lleva sabor"));
            Assert.Contains(r.Errores, x => x.Contains("Papas") && x.Contains("repetido"));
            Assert.Equal(0, ContarProductos(e, "Papas mal cargadas"));
        }

        [Fact]
        public async Task Crear_SiElProductoEsInvalido_TampocoQuedaLaReceta()
        {
            using var e = new Escenario();
            var producto = Nuevo("Sin categoría");
            producto.IdCategoriaProducto = 999;

            var r = await Alta(e).CrearConRecetaAsync(producto, new List<LineaIngrediente> { Ing(Papas, 0.30m) }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("categoría"));
            using var ctx = e.NuevoContexto();
            Assert.Equal(0, ctx.Productos.Count(p => p.Nombre == "Sin categoría"));
            Assert.Equal(0, ctx.Recetas.Count(x => x.IdProducto == 0 || x.Producto!.Nombre == "Sin categoría"));
        }

        [Fact]
        public async Task Crear_ConSabor_LaLineaSoloAplicaAEseSabor()
        {
            using var e = new Escenario();
            var producto = Nuevo("Alitas de prueba", tipo: 1, sabor: true);
            producto.IdCategoriaProducto = 1;

            var r = await Alta(e).CrearConRecetaAsync(producto, new List<LineaIngrediente>
            {
                Ing(1, 0.50m),                 // alitas: todos los sabores
                Ing(SalsaBbq, 0.05m, sabor: 2) // BBQ solo si se pide BBQ
            }, Admin);
            Assert.True(r.Exito, string.Join(" | ", r.Errores));

            await e.RegistrarAsync(Salon(6, Linea(producto.IdProducto, 1, idSabor: 1)));   // Buffalo
            Assert.Equal(4m, e.Stock(SalsaBbq));                                             // no tocó la BBQ
            await e.RegistrarAsync(Salon(7, Linea(producto.IdProducto, 1, idSabor: 2)));   // BBQ
            Assert.Equal(3.95m, e.Stock(SalsaBbq));
        }

        [Fact]
        public async Task Crear_Combo_ConIngredientes_SeRechaza()
        {
            using var e = new Escenario();
            var combo = Nuevo("Combo de prueba", tipo: Producto.IdTipoCombo);

            var r = await Alta(e).CrearConRecetaAsync(combo, new List<LineaIngrediente> { Ing(Papas, 1m) }, Admin);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("se arma con productos"));
            Assert.Equal(0, ContarProductos(e, "Combo de prueba"));
        }
    }
}
