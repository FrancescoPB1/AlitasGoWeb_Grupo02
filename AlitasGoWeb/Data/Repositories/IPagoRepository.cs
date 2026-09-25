using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    // Consultas de los pagos ya registrados (solo lectura).
    public interface IPagoRepository
    {
        Task<List<Pago>> ListarAsync(DateTime desde, DateTime hastaExclusivo, int? idTipoPago, string? tipoComprobante);
        Task<Pago?> ObtenerAsync(int idPedido);
    }
}
