using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    // Historial de pagos: solo consulta. Los pagos se crean en PedidoService.CobrarAsync.
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _repo;

        public PagoService(IPagoRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Pago>> ListarAsync(DateTime desde, DateTime hasta, int? idTipoPago, string? tipoComprobante)
        {
            // "hasta" incluye todo ese día: se busca hasta antes de la medianoche siguiente.
            return _repo.ListarAsync(desde.Date, hasta.Date.AddDays(1), idTipoPago, tipoComprobante);
        }

        public Task<Pago?> ObtenerAsync(int idPedido) => _repo.ObtenerAsync(idPedido);
    }
}
