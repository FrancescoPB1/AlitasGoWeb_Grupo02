using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Tests.Infraestructura;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Cada operación exitosa del pedido emite un aviso; si falla, no se avisa nada.
    public class AvisosTiempoRealTests
    {
        [Fact]
        public async Task CicloDelPedido_EmiteUnAvisoPorCadaCambio()
        {
            using var e = new Escenario();

            var id = await e.RegistrarAsync(Salon(3, Linea(9, 1)));
            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo);

            Assert.Equal(new[] { EventosPedido.Registrado, EventosPedido.Estado, EventosPedido.Estado },
                         e.Notificador.Avisos.Select(a => a.Evento).ToArray());
            var listo = e.Notificador.Avisos.Last();
            Assert.Equal(id, listo.IdPedido);
            Assert.Equal(Estados.Listo, listo.IdEstado);
            Assert.Equal(Canales.Salon, listo.IdCanal);
            Assert.Equal(3, listo.NumeroMesa);
        }

        [Fact]
        public async Task AnularYCobrar_TambienAvisan()
        {
            using var e = new Escenario();
            var anulado = await e.RegistrarAsync(Salon(1, Linea(9, 1)));
            await e.Pedidos.AnularAsync(anulado, "Cliente se fue", Admin);

            var entregado = await e.RegistrarAsync(Delivery(1, Linea(9, 1)));
            await e.LlevarAAsync(entregado, Estados.EnPreparacion, Estados.Listo, Estados.EnReparto);
            await e.Pedidos.CobrarAsync(entregado, new Services.Dtos.SolicitudCobro { IdTipoPago = 1 }, Usuario);

            Assert.Contains(e.Notificador.Avisos, a => a.IdPedido == anulado && a.Evento == EventosPedido.Anulado && a.IdEstado == Estados.Anulado);
            Assert.Contains(e.Notificador.Avisos, a => a.IdPedido == entregado && a.Evento == EventosPedido.Cobrado && a.IdEstado == Estados.Entregado);
        }

        [Fact]
        public async Task OperacionRechazada_NoEmiteAvisos()
        {
            using var e = new Escenario();

            await e.Pedidos.RegistrarAsync(Salon(1, Linea(2, 1)), Usuario);   // falta el sabor
            await e.Pedidos.CambiarEstadoAsync(999, Estados.Listo, Usuario);  // no existe

            Assert.Empty(e.Notificador.Avisos);
        }
    }
}
