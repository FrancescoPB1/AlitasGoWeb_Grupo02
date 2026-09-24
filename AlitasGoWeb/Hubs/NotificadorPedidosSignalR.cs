using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.SignalR;

namespace AlitasGoWeb.Hubs
{
    public record MensajeTiempoReal(int IdPedido, int IdCanal, int IdEstado, string Estado, string Evento, string? Mensaje);

    public class NotificadorPedidosSignalR : INotificadorPedidos
    {
        private readonly IHubContext<PedidosHub> _hub;
        private readonly ILogger<NotificadorPedidosSignalR> _logger;

        public NotificadorPedidosSignalR(IHubContext<PedidosHub> hub, ILogger<NotificadorPedidosSignalR> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task PedidoActualizadoAsync(AvisoPedido aviso)
        {
            try
            {
                foreach (var (grupo, mensaje) in Destinos(aviso))
                {
                    await _hub.Clients.Group(grupo).SendAsync(PedidosHub.Metodo, new MensajeTiempoReal(
                        aviso.IdPedido, aviso.IdCanal, aviso.IdEstado,
                        MaquinaEstadosPedido.Nombre(aviso.IdEstado), aviso.Evento, mensaje));
                }
            }
            catch (Exception ex)
            {
                // El pedido ya se guardó: un fallo del aviso no debe deshacer la operación.
                _logger.LogWarning(ex, "No se pudo notificar en tiempo real el pedido {IdPedido}.", aviso.IdPedido);
            }
        }

        /// <summary>
        /// A qué grupos (roles) les interesa el cambio y qué aviso ve cada uno (null = solo refrescar la pantalla).
        /// </summary>
        public static Dictionary<string, string?> Destinos(AvisoPedido a)
        {
            var esDelivery = a.IdCanal == Canales.Delivery;
            var mesa = a.NumeroMesa.HasValue ? $" · Mesa {a.NumeroMesa}" : "";
            var destinos = new Dictionary<string, string?>();

            // Caja: todo lo del día; avisa cuando hay algo que hacer.
            string? caja = (a.Evento, a.IdEstado) switch
            {
                (EventosPedido.Estado, Estados.Listo) when !esDelivery => $"Pedido #{a.IdPedido} listo para servir{mesa}",
                (EventosPedido.Anulado, _) => $"Pedido #{a.IdPedido} anulado",
                (EventosPedido.Cobrado, _) when esDelivery => $"Delivery #{a.IdPedido} entregado y cobrado",
                _ => null
            };
            destinos[GruposTiempoReal.Caja] = caja;

            // Cocina: pedidos que entran, cambian o salen de su tablero. Su pantalla lee el aviso en voz alta.
            var afectaCocina = a.Evento is EventosPedido.Registrado or EventosPedido.Editado or EventosPedido.Anulado
                               || a.IdEstado is Estados.EnPreparacion or Estados.Listo;
            if (afectaCocina)
            {
                destinos[GruposTiempoReal.Cocina] = a.Evento switch
                {
                    EventosPedido.Anulado => $"Atención: pedido #{a.IdPedido} anulado",
                    EventosPedido.Editado => $"Atención: el pedido #{a.IdPedido} fue modificado",
                    _ => null   // los nuevos se anuncian con su detalle al refrescar el tablero
                };
            }

            // Reparto: solo delivery, cuando ya está listo o después.
            if (esDelivery && (a.IdEstado is Estados.Listo or Estados.EnReparto or Estados.Entregado or Estados.Anulado))
            {
                destinos[GruposTiempoReal.Reparto] = (a.Evento, a.IdEstado) switch
                {
                    (EventosPedido.Estado, Estados.Listo) => $"Pedido #{a.IdPedido} listo para despachar",
                    (EventosPedido.Anulado, _) => $"Pedido #{a.IdPedido} anulado: no salir",
                    _ => null
                };
            }

            // Administradora: ve todo, con los avisos de caja y reparto.
            destinos[GruposTiempoReal.Admin] = caja
                ?? (esDelivery && a.Evento == EventosPedido.Estado && a.IdEstado == Estados.Listo
                    ? $"Pedido #{a.IdPedido} listo para despachar" : null);

            return destinos;
        }
    }
}
