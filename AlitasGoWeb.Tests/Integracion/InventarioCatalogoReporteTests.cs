using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    public class InventarioCatalogoReporteTests
    {
        [Fact]
        public async Task RegistrarCompra_SumaStockGuardaCompraYMovimientoDeEntrada()
        {
            using var e = new Escenario();

            var r = await e.Inventario.RegistrarCompraAsync(new List<LineaCompra>
            {
                new() { IdInsumo = 1, Cantidad = 10m, CostoUnitario = 12.50m },
                new() { IdInsumo = 3, Cantidad = 2m, CostoUnitario = 15m }
            }, Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            Assert.Equal(30m, e.Stock(1));
            Assert.Equal(6m, e.Stock(3));

            using var ctx = e.NuevoContexto();
            var compra = Assert.Single(ctx.ComprasInsumos);
            Assert.Equal(155m, compra.Total);
            Assert.Equal(2, ctx.MovimientosInventario.Count(m => m.Tipo == TiposMovimiento.Entrada));
            Assert.Single(ctx.Auditorias.Where(a => a.Accion == "Registrar compra"));
        }

        [Fact]
        public async Task RegistrarCompra_ConCantidadCero_NoSeGuarda()
        {
            using var e = new Escenario();

            var r = await e.Inventario.RegistrarCompraAsync(new List<LineaCompra> { new() { IdInsumo = 1, Cantidad = 0m } }, Admin);

            Assert.False(r.Exito);
            Assert.Equal(20m, e.Stock(1));
        }

        [Fact]
        public async Task CambiarPrecio_QuedaRegistradoEnLaBitacoraConValorAnteriorYNuevo()
        {
            using var e = new Escenario();
            var datos = (await e.Catalogo.ObtenerProductoAsync(2))!;
            var edicion = new Producto
            {
                IdProducto = 2, Nombre = datos.Nombre, IdCategoriaProducto = datos.IdCategoriaProducto,
                IdTipoProducto = datos.IdTipoProducto, Descripcion = datos.Descripcion,
                Precio = 32m, RequiereSabor = true, Activo = true
            };
            e.Db.ChangeTracker.Clear();

            var r = await e.Catalogo.ActualizarProductoAsync(edicion, Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            using var ctx = e.NuevoContexto();
            Assert.Equal(32m, ctx.Productos.Single(p => p.IdProducto == 2).Precio);
            var registro = Assert.Single(ctx.Auditorias.Where(a => a.Accion == "Cambiar precio"));
            Assert.Contains("30", registro.Detalle);   // valor anterior
            Assert.Contains("32", registro.Detalle);   // valor nuevo
        }

        [Fact]
        public async Task DesactivarCategoria_ConProductosActivos_NoSePermite()
        {
            using var e = new Escenario();

            var r = await e.Catalogo.DesactivarCategoriaAsync(1, Admin);

            Assert.False(r.Exito);
            Assert.True(e.NuevoContexto().CategoriasProducto.Single(c => c.IdCategoriaProducto == 1).Activo);
        }

        [Fact]
        public async Task CrearCategoria_NombreRepetido_NoSePermite()
        {
            using var e = new Escenario();
            var existente = e.NuevoContexto().CategoriasProducto.First().Nombre;

            var r = await e.Catalogo.CrearCategoriaAsync(new CategoriaProducto { Nombre = existente }, Admin);

            Assert.False(r.Exito);
        }

        [Fact]
        public async Task CrearCliente_DniDuplicado_NoSePermite()
        {
            using var e = new Escenario();

            var r = await e.Catalogo.CrearClienteAsync(new Cliente { DNI = "12345678", Nombre = "Otro", ApellidoPaterno = "Cliente", ApellidoMaterno = "" });

            Assert.False(r.Exito);
        }

        [Fact]
        public async Task Receta_AgregarLinea_AfectaElConsumoDelSiguientePedido()
        {
            using var e = new Escenario();
            // Las yucas (producto 7) aún no descuentan nada: se les agrega 0.3 kg de papas como ejemplo
            var r = await e.Inventario.AgregarLineaRecetaAsync(7, 6, null, 0.3m, Admin);
            Assert.True(r.Exito, string.Join(" | ", r.Errores));

            await e.RegistrarAsync(Salon(1, Linea(7, 2)));

            Assert.Equal(14.4m, e.Stock(6));
        }

        [Fact]
        public async Task Reporte_SoloCuentaEntregadosYSeparaSalonDeDelivery()
        {
            using var e = new Escenario();
            var salon = await e.RegistrarAsync(Salon(1, Linea(2, 1, idSabor: 2), Linea(9, 1)));      // 35
            var delivery = await e.RegistrarAsync(Delivery(1, Linea(1, 2, idSabor: 2)));           // 36 + 3
            var anulado = await e.RegistrarAsync(Salon(2, Linea(1, 1, idSabor: 1)));
            var enCurso = await e.RegistrarAsync(Salon(3, Linea(6, 1)));

            await e.LlevarAAsync(salon, Estados.EnPreparacion, Estados.Listo, Estados.Servido);
            Assert.True((await e.Pedidos.CobrarAsync(salon, new SolicitudCobro { IdTipoPago = 1 }, Usuario)).Exito);
            await e.LlevarAAsync(delivery, Estados.EnPreparacion, Estados.Listo, Estados.EnReparto);
            Assert.True((await e.Pedidos.CobrarAsync(delivery, new SolicitudCobro { IdTipoPago = 2 }, Usuario)).Exito);
            Assert.True((await e.Pedidos.AnularAsync(anulado, "Prueba de reporte", Admin)).Exito);

            var reporte = await e.Reportes.GenerarAsync(Dias.Lunes, Dias.Lunes);

            Assert.Equal(74m, reporte.TotalVendido);
            Assert.Equal(2, reporte.PedidosEntregados);
            Assert.Equal(1, reporte.PedidosAnulados);
            Assert.Equal(1, reporte.PedidosEnCurso);
            Assert.Equal(37m, reporte.TicketPromedio);
            Assert.Equal(2, reporte.PorCanal.Count);
            Assert.Equal(39m, reporte.PorCanal.Single(c => c.Canal == "Delivery").Total);
            var bbq = reporte.Sabores.First();
            Assert.Equal("BBQ", bbq.Sabor);
            Assert.Equal(3, bbq.Porciones);
            Assert.DoesNotContain(reporte.Sabores, s => s.Sabor == "Buffalo");   // el de Buffalo se anuló
            _ = enCurso;
        }
    }
}
