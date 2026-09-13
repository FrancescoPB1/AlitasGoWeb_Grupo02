using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface IPedidoService
    {
        Task<int> RegistrarPedidoAsync(Pedido pedido, List<DetallePedido> detalles);
        Task<Pedido?> ObtenerConDetallesAsync(int idPedido);
        Task<List<Pedido>> ListarAsync();
        Task CambiarEstadoAsync(int idPedido, int nuevoEstadoId);
        Task AnularAsync(int idPedido);
    }
}
