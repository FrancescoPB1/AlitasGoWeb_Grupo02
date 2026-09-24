using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // "Cuánto vendió, qué sabores salieron más y cómo le fue en el salón frente al delivery".
    // Solo cuentan como venta los pedidos Entregados (cobrados).
    public class ReporteService : IReporteService
    {
        private readonly IPedidoRepository _repo;

        public ReporteService(IPedidoRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReporteVentas> GenerarAsync(DateTime desde, DateTime hasta)
        {
            if (hasta < desde) (desde, hasta) = (hasta, desde);
            var pedidos = await _repo.ListarParaReporteAsync(desde.Date, hasta.Date.AddDays(1));
            var entregados = pedidos.Where(p => p.IdEstadoPedido == Estados.Entregado).ToList();
            var detalles = entregados.SelectMany(p => p.DetallePedidos).ToList();

            var total = entregados.Sum(p => p.Total);
            return new ReporteVentas
            {
                Desde = desde.Date,
                Hasta = hasta.Date,
                TotalVendido = total,
                PedidosEntregados = entregados.Count,
                PedidosAnulados = pedidos.Count(p => p.IdEstadoPedido == Estados.Anulado),
                PedidosEnCurso = pedidos.Count(p => p.IdEstadoPedido != Estados.Entregado && p.IdEstadoPedido != Estados.Anulado),
                TicketPromedio = entregados.Count == 0 ? 0 : Math.Round(total / entregados.Count, 2),
                PorCanal = entregados
                    .GroupBy(p => p.CanalAtencion?.Nombre ?? "—")
                    .Select(g => new VentaPorCanal(g.Key, g.Count(), g.Sum(p => p.Total)))
                    .OrderByDescending(v => v.Total).ToList(),
                Sabores = detalles
                    .Where(d => d.Sabor != null)
                    .GroupBy(d => d.Sabor!.Nombre)
                    .Select(g => new SaborVendido(g.Key, g.Sum(d => d.Cantidad)))
                    .OrderByDescending(s => s.Porciones).ThenBy(s => s.Sabor).ToList(),
                Productos = detalles
                    .GroupBy(d => d.Producto?.Nombre ?? "—")
                    .Select(g => new ProductoVendido(g.Key, g.Sum(d => d.Cantidad), g.Sum(d => d.SubTotal)))
                    .OrderByDescending(p => p.Cantidad).ThenBy(p => p.Producto).ToList(),
                PorTipoPago = entregados
                    .Where(p => p.Pago != null)
                    .GroupBy(p => p.Pago!.TipoPago?.Nombre ?? "—")
                    .Select(g => new VentaPorTipoPago(g.Key, g.Count(), g.Sum(p => p.Total)))
                    .OrderByDescending(v => v.Total).ToList()
            };
        }
    }
}
