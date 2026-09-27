using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using AlitasGoWeb.ViewModels;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Correcciones reportadas por el equipo: mesa ocupada, cambio de mesa y anulación de un pedido ya cobrado.
    public class CorreccionesTests
    {
        private static async Task<int> CobradoEnMesaAsync(Escenario e, int mesa)
        {
            var id = await e.RegistrarAsync(Salon(mesa, Linea(2, 1, idSabor: 1)));   // Docena Buffalo
            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo, Estados.Servido);
            var r = await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 2, TipoComprobante = TiposComprobante.Boleta }, Usuario);
            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            return id;
        }

        // ---------- Mesa ocupada ----------
        [Fact]
        public async Task MesaOcupada_NoAceptaOtroPedido_HastaQueSeCobra()
        {
            using var e = new Escenario();
            var primero = await e.RegistrarAsync(Salon(3, Linea(9, 1)));

            var segundo = await e.Pedidos.RegistrarAsync(Salon(3, Linea(9, 1)), Usuario);
            Assert.False(segundo.Exito);
            Assert.Contains(segundo.Errores, x => x.Contains("mesa 3 está ocupada") && x.Contains($"#{primero}"));

            // Otra mesa sí se puede usar.
            Assert.True((await e.Pedidos.RegistrarAsync(Salon(4, Linea(9, 1)), Usuario)).Exito);

            // Cobrado el primero, la mesa 3 queda libre otra vez.
            await e.LlevarAAsync(primero, Estados.EnPreparacion, Estados.Listo, Estados.Servido);
            Assert.True((await e.Pedidos.CobrarAsync(primero, new SolicitudCobro { IdTipoPago = 1 }, Usuario)).Exito);
            Assert.True((await e.Pedidos.RegistrarAsync(Salon(3, Linea(9, 1)), Usuario)).Exito);
        }

        [Fact]
        public async Task MesaConPedidoAnulado_QuedaLibre()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(5, Linea(9, 1)));
            Assert.True((await e.Pedidos.AnularAsync(id, "El cliente se fue", Admin)).Exito);

            Assert.True((await e.Pedidos.RegistrarAsync(Salon(5, Linea(9, 1)), Usuario)).Exito);
        }

        // ---------- Cambiar mesa al editar ----------
        [Fact]
        public async Task Editar_PuedeCambiarASoloUnaMesaLibre()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(1, Linea(9, 1)));
            await e.RegistrarAsync(Salon(2, Linea(9, 1)));   // la mesa 2 queda ocupada

            // Agregar productos sin cambiar de mesa: su propia mesa no cuenta como ocupada.
            Assert.True((await e.Pedidos.EditarAsync(id, Salon(1, Linea(9, 2)), Usuario)).Exito);

            var aOcupada = await e.Pedidos.EditarAsync(id, Salon(2, Linea(9, 2)), Usuario);
            Assert.False(aOcupada.Exito);
            Assert.Contains(aOcupada.Errores, x => x.Contains("mesa 2 está ocupada"));

            Assert.True((await e.Pedidos.EditarAsync(id, Salon(6, Linea(9, 2)), Usuario)).Exito);
            Assert.Equal(6, e.Leer(id).PedidoLocal!.IdNroMesa);
        }

        // ---------- Anular un pedido ya cobrado ----------
        [Fact]
        public async Task AnularCobrado_AnulaElComprobante_NoRepoStock_YQuedaEnLaBitacora()
        {
            using var e = new Escenario();
            var id = await CobradoEnMesaAsync(e, 7);
            var alitasAntes = e.Stock(1);

            var r = await e.Pedidos.AnularAsync(id, "Se cobró el producto equivocado", Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            Assert.Contains($"B001-{id:D8}", r.Mensaje);
            var pedido = e.Leer(id);
            Assert.Equal(Estados.Anulado, pedido.IdEstadoPedido);
            Assert.NotNull(pedido.Pago);                        // el pago histórico se conserva
            Assert.Equal(alitasAntes, e.Stock(1));              // la comida ya se sirvió: no vuelve al stock

            using var ctx = e.NuevoContexto();
            var registro = Assert.Single(ctx.Auditorias.Where(a => a.Accion == "Anular pedido" && a.IdEntidad == id.ToString()));
            Assert.Equal(Admin, registro.Usuario);
            Assert.Contains("Comprobante anulado", registro.Detalle);
            Assert.Contains("Se cobró el producto equivocado", registro.Detalle);
        }

        [Fact]
        public async Task AnularCobrado_DejaDeContarEnVentasEIngresos()
        {
            using var e = new Escenario(Dias.Lunes);
            var vigente = await CobradoEnMesaAsync(e, 1);
            var anulado = await CobradoEnMesaAsync(e, 2);
            Assert.True((await e.Pedidos.AnularAsync(anulado, "Cliente se retractó", Admin)).Exito);

            var reporte = await e.Reportes.GenerarAsync(Dias.Lunes, Dias.Lunes);
            Assert.Equal(1, reporte.PedidosEntregados);
            Assert.Equal(1, reporte.PedidosAnulados);

            using var ctx = e.NuevoContexto();
            var vm = new HistorialPagosVM { Pagos = await new PagoService(new PagoRepository(ctx)).ListarAsync(Dias.Lunes, Dias.Lunes, null, null) };
            Assert.Equal(1, vm.Cantidad);
            Assert.Equal(1, vm.Anulados);
            Assert.Equal(e.Leer(vigente).Total, vm.TotalCobrado);
        }
    }
}
