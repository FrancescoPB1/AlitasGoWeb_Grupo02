using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObtenerConDetallesAsync(int idPedido);
        Task<List<Pedido>> ObtenerTodosAsync();
        Task AgregarAsync(Pedido pedido);
        Task<int> GuardarAsync();
    }
}
