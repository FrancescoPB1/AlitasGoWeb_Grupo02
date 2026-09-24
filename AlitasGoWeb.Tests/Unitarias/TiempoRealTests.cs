using AlitasGoWeb.Hubs;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;

namespace AlitasGoWeb.Tests.Unitarias
{
    // A quién le llega cada aviso en tiempo real y qué mensaje ve.
    public class TiempoRealTests
    {
        private static Dictionary<string, string?> Destinos(int canal, int estado, string evento, int? mesa = null) =>
            NotificadorPedidosSignalR.Destinos(new AvisoPedido(12, canal, estado, evento, mesa));

        [Fact]
        public void PedidoNuevo_RefrescaCocinaYCajaSinMensaje()
        {
            var d = Destinos(Canales.Salon, Estados.Recibido, EventosPedido.Registrado);

            Assert.True(d.ContainsKey(GruposTiempoReal.Cocina));
            Assert.Null(d[GruposTiempoReal.Cocina]);   // la cocina lo anuncia con su detalle al refrescar
            Assert.Null(d[GruposTiempoReal.Caja]);
            Assert.False(d.ContainsKey(GruposTiempoReal.Reparto));
        }

        [Fact]
        public void SalonListo_AvisaACajaConLaMesa_YNoAReparto()
        {
            var d = Destinos(Canales.Salon, Estados.Listo, EventosPedido.Estado, mesa: 3);

            Assert.Equal("Pedido #12 listo para servir · Mesa 3", d[GruposTiempoReal.Caja]);
            Assert.Equal(d[GruposTiempoReal.Caja], d[GruposTiempoReal.Admin]);
            Assert.False(d.ContainsKey(GruposTiempoReal.Reparto));
        }

        [Fact]
        public void DeliveryListo_AvisaAlRepartidor()
        {
            var d = Destinos(Canales.Delivery, Estados.Listo, EventosPedido.Estado);

            Assert.Equal("Pedido #12 listo para despachar", d[GruposTiempoReal.Reparto]);
            Assert.Equal("Pedido #12 listo para despachar", d[GruposTiempoReal.Admin]);
            Assert.Null(d[GruposTiempoReal.Caja]);
        }

        [Fact]
        public void DeliveryRecibido_NoMolestaAlRepartidor()
        {
            Assert.False(Destinos(Canales.Delivery, Estados.Recibido, EventosPedido.Registrado).ContainsKey(GruposTiempoReal.Reparto));
        }

        [Fact]
        public void Anulado_AvisaALaCocinaParaQueNoLoPrepare()
        {
            var d = Destinos(Canales.Salon, Estados.Anulado, EventosPedido.Anulado);

            Assert.Equal("Atención: pedido #12 anulado", d[GruposTiempoReal.Cocina]);
            Assert.Equal("Pedido #12 anulado", d[GruposTiempoReal.Caja]);
        }
    }
}
