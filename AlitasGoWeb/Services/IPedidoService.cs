using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public interface IPedidoService
    {
        Task<Resultado<int>> RegistrarAsync(SolicitudPedido solicitud, string usuario);
        Task<Resultado> EditarAsync(int idPedido, SolicitudPedido solicitud, string usuario);
        Task<Pedido?> ObtenerConDetallesAsync(int idPedido);
        Task<List<Pedido>> ListarAsync(DateTime? fecha, int? idEstado);
        Task<List<Pedido>> ListarCocinaAsync();
        Task<List<Pedido>> ListarRepartoAsync();
        Task<List<Pedido>> ListarActivosSalonAsync();
        Task<Resultado> CambiarEstadoAsync(int idPedido, int nuevoEstado, string usuario);
        Task<Resultado> AnularAsync(int idPedido, string? motivo, string usuario);
        Task<Resultado> CobrarAsync(int idPedido, SolicitudCobro cobro, string usuario);
    }
}
