using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IPromocionRepository
    {
        Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanalAtencion, int idDia);
        Task<List<Promocion>> ListarAsync();
        Task<Promocion?> ObtenerAsync(int idPromocion);
        Task AgregarAsync(Promocion promocion);
        Task<List<TipoPromocion>> ListarTiposAsync();
        Task<List<CanalPromocion>> ListarCanalesAsync();
        Task<List<Dia>> ListarDiasAsync();
        Task<List<Producto>> ObtenerProductosAsync(IEnumerable<int> ids);
    }
}
