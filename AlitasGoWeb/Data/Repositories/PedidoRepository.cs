using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    // FABRICACIÓN PURA + INDIRECCIÓN: aísla a los servicios de EF Core.
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public PedidoRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Pedido?> ObtenerConDetallesAsync(int idPedido)
        {
            return _ctx.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.CanalAtencion)
                .Include(p => p.EstadoPedido)
                .Include(p => p.PedidoDelivery)
                    .ThenInclude(d => d!.ZonaDelivery)
                .Include(p => p.PedidoLocal)
                    .ThenInclude(l => l!.NroMesa)
                        .ThenInclude(m => m!.ZonaLocal)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Sabor)
                .Include(p => p.Pago)
                    .ThenInclude(pg => pg!.TipoPago)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
        }

        public Task<List<Pedido>> ListarAsync(DateTime? desde, DateTime? hastaExclusivo, int? idEstado)
        {
            var q = _ctx.Pedidos.AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.CanalAtencion)
                .Include(p => p.EstadoPedido)
                .Include(p => p.PedidoLocal)
                    .ThenInclude(l => l!.NroMesa)
                .Include(p => p.PedidoDelivery)
                    .ThenInclude(d => d!.ZonaDelivery)
                .AsQueryable();

            if (desde.HasValue) q = q.Where(p => p.Fecha >= desde.Value);
            if (hastaExclusivo.HasValue) q = q.Where(p => p.Fecha < hastaExclusivo.Value);
            if (idEstado.HasValue) q = q.Where(p => p.IdEstadoPedido == idEstado.Value);

            return q.OrderByDescending(p => p.Fecha).ToListAsync();
        }

        // Cocina, salón y reparto: el más antiguo primero (FIFO).
        public Task<List<Pedido>> ListarPorEstadosAsync(IEnumerable<int> estados, int? idCanal)
        {
            var ids = estados.ToList();
            var q = _ctx.Pedidos.AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.EstadoPedido)
                .Include(p => p.PedidoLocal)
                    .ThenInclude(l => l!.NroMesa)
                .Include(p => p.PedidoDelivery)
                    .ThenInclude(d => d!.ZonaDelivery)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Sabor)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(pr => pr!.Componentes)
                            .ThenInclude(c => c.ProductoComponente)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(pr => pr!.Componentes)
                            .ThenInclude(c => c.Sabor)
                .Where(p => ids.Contains(p.IdEstadoPedido));

            if (idCanal.HasValue) q = q.Where(p => p.IdCanalAtencion == idCanal.Value);

            return q.OrderBy(p => p.Fecha).ToListAsync();
        }

        public Task<List<Pedido>> ListarParaReporteAsync(DateTime desde, DateTime hastaExclusivo)
        {
            return _ctx.Pedidos.AsNoTracking()
                .Include(p => p.CanalAtencion)
                .Include(p => p.Pago)
                    .ThenInclude(pg => pg!.TipoPago)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Sabor)
                .Where(p => p.Fecha >= desde && p.Fecha < hastaExclusivo)
                .ToListAsync();
        }

        public async Task AgregarAsync(Pedido pedido)
        {
            await _ctx.Pedidos.AddAsync(pedido);
        }

        public void EliminarDetalles(IEnumerable<DetallePedido> detalles)
        {
            _ctx.DetallesPedido.RemoveRange(detalles);
        }
    }
}
