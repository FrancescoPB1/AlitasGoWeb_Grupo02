namespace AlitasGoWeb.Services
{
    public static class EventosPedido
    {
        public const string Registrado = "registrado";
        public const string Editado = "editado";
        public const string Estado = "estado";
        public const string Anulado = "anulado";
        public const string Cobrado = "cobrado";
    }

    public record AvisoPedido(int IdPedido, int IdCanal, int IdEstado, string Evento, int? NumeroMesa);

    // Los servicios avisan "algo cambió en este pedido" sin saber cómo llega a las pantallas
    // (inversión de dependencias: la implementación con SignalR vive en la capa de presentación).
    public interface INotificadorPedidos
    {
        Task PedidoActualizadoAsync(AvisoPedido aviso);
    }
}
