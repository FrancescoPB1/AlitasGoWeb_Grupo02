using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface IPagoService
    {
        Task<List<Pago>> ListarAsync(DateTime desde, DateTime hasta, int? idTipoPago, string? tipoComprobante);
        Task<Pago?> ObtenerAsync(int idPedido);
    }
}
