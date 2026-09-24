using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public interface IPromocionService
    {
        Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanal, DateTime fecha);
        Task<List<Promocion>> ListarAsync();
        Task<Resultado> CrearAsync(SolicitudPromocion solicitud, string usuario);
        Task<Resultado> AlternarActivoAsync(int idPromocion, string usuario);
        Task<List<TipoPromocion>> ListarTiposAsync();
        Task<List<CanalPromocion>> ListarCanalesAsync();
        Task<List<Dia>> ListarDiasAsync();
    }
}
