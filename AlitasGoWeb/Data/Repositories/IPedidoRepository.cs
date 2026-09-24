using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObtenerConDetallesAsync(int idPedido);
        Task<List<Pedido>> ListarAsync(DateTime? desde, DateTime? hastaExclusivo, int? idEstado);
        Task<List<Pedido>> ListarPorEstadosAsync(IEnumerable<int> estados, int? idCanal);
        Task<List<Pedido>> ListarParaReporteAsync(DateTime desde, DateTime hastaExclusivo);
        Task AgregarAsync(Pedido pedido);
        void EliminarDetalles(IEnumerable<DetallePedido> detalles);
    }
}
