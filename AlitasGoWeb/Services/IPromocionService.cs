namespace AlitasGoWeb.Services
{
    public interface IPromocionService
    {
        Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanal, DateTime fecha);
    }
}
