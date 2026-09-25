using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Historial de pagos: lista todos los cobros y su detalle, con filtros.
    public class HistorialPagosTests
    {
        // Registra un pedido en salón, lo lleva hasta "Servido" y lo cobra.
        private static async Task<int> CobrarSalonAsync(Escenario e, int mesa, int idTipoPago, string comprobante)
        {
            var id = await e.RegistrarAsync(Salon(mesa, Linea(2, 1, idSabor: 1)));
            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo, Estados.Servido);
            var r = await e.Pedidos.CobrarAsync(id, new SolicitudCobro
            {
                IdTipoPago = idTipoPago,
                TipoComprobante = comprobante,
                Ruc = comprobante == TiposComprobante.Factura ? "20481234567" : null,
                RazonSocial = comprobante == TiposComprobante.Factura ? "Eventos Trujillo SAC" : null
            }, Usuario);
            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            return id;
        }

        [Fact]
        public async Task Listar_MuestraSoloLosPedidosCobradosYElMasRecienteArriba()
        {
            using var e = new Escenario(Dias.Lunes);
            var boleta = await CobrarSalonAsync(e, 1, 1, TiposComprobante.Boleta);
            e.Reloj.Ahora = Dias.Lunes.AddHours(1);
            var factura = await CobrarSalonAsync(e, 2, 2, TiposComprobante.Factura);
            await e.RegistrarAsync(Salon(3, Linea(2, 1, idSabor: 1)));   // sin cobrar: no debe salir

            using var ctx = e.NuevoContexto();
            var pagos = new PagoService(new PagoRepository(ctx));
            var lista = await pagos.ListarAsync(Dias.Lunes, Dias.Lunes, null, null);

            Assert.Equal(new[] { factura, boleta }, lista.Select(p => p.IdPedido));
            Assert.All(lista, p => Assert.NotNull(p.TipoPago));
            Assert.All(lista, p => Assert.NotNull(p.Pedido!.Cliente));
        }

        [Fact]
        public async Task Listar_FiltraPorFechaMedioDePagoYComprobante()
        {
            using var e = new Escenario(Dias.Lunes);
            var lunes = await CobrarSalonAsync(e, 1, 1, TiposComprobante.Boleta);
            e.Reloj.Ahora = Dias.Martes;
            var martes = await CobrarSalonAsync(e, 2, 2, TiposComprobante.Factura);

            using var ctx = e.NuevoContexto();
            var pagos = new PagoService(new PagoRepository(ctx));

            Assert.Equal(new[] { martes }, (await pagos.ListarAsync(Dias.Martes, Dias.Martes, null, null)).Select(p => p.IdPedido));
            Assert.Equal(new[] { lunes }, (await pagos.ListarAsync(Dias.Lunes, Dias.Martes, 1, null)).Select(p => p.IdPedido));
            Assert.Equal(new[] { martes }, (await pagos.ListarAsync(Dias.Lunes, Dias.Martes, null, TiposComprobante.Factura)).Select(p => p.IdPedido));
            Assert.Empty(await pagos.ListarAsync(Dias.Miercoles, Dias.Jueves, null, null));
        }

        [Fact]
        public async Task Obtener_TraeElPagoConLosProductosDelPedido()
        {
            using var e = new Escenario(Dias.Lunes);
            var id = await CobrarSalonAsync(e, 4, 1, TiposComprobante.Boleta);

            using var ctx = e.NuevoContexto();
            var pagos = new PagoService(new PagoRepository(ctx));
            var pago = await pagos.ObtenerAsync(id);

            Assert.NotNull(pago);
            Assert.Equal($"B001-{id:D8}", pago!.NumeroComprobante);
            Assert.Equal(Usuario, pago.UsuarioCobro);
            var linea = Assert.Single(pago.Pedido!.DetallePedidos);
            Assert.NotNull(linea.Producto);
            Assert.Equal(4, pago.Pedido.PedidoLocal!.NroMesa!.NumeroMesa);
            Assert.Null(await pagos.ObtenerAsync(9999));
        }
    }
}
