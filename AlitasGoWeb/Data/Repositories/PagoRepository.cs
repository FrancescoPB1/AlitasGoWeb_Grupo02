using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public PagoRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        // Lista de pagos en un rango de fechas, con filtros opcionales. El más reciente primero.
        public Task<List<Pago>> ListarAsync(DateTime desde, DateTime hastaExclusivo, int? idTipoPago, string? tipoComprobante)
        {
            var q = _ctx.Pagos.AsNoTracking()
                .Include(pg => pg.TipoPago)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.Cliente)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.CanalAtencion)
                .Where(pg => pg.FechaPago >= desde && pg.FechaPago < hastaExclusivo);

            if (idTipoPago.HasValue)
                q = q.Where(pg => pg.IdTipoPago == idTipoPago.Value);

            if (!string.IsNullOrEmpty(tipoComprobante))
                q = q.Where(pg => pg.TipoComprobante == tipoComprobante);

            return q.OrderByDescending(pg => pg.FechaPago).ToListAsync();
        }

        // Un pago con todo su pedido: productos, sabores, cliente y envío.
        public Task<Pago?> ObtenerAsync(int idPedido)
        {
            return _ctx.Pagos.AsNoTracking()
                .Include(pg => pg.TipoPago)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.Cliente)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.CanalAtencion)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.PedidoLocal)
                        .ThenInclude(l => l!.NroMesa)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.PedidoDelivery)
                        .ThenInclude(d => d!.ZonaDelivery)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.DetallePedidos)
                        .ThenInclude(d => d.Producto)
                .Include(pg => pg.Pedido)
                    .ThenInclude(p => p!.DetallePedidos)
                        .ThenInclude(d => d.Sabor)
                .FirstOrDefaultAsync(pg => pg.IdPedido == idPedido);
        }
    }
}
