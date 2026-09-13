using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
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
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Sabor)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
        }

        public Task<List<Pedido>> ObtenerTodosAsync()
        {
            return _ctx.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.CanalAtencion)
                .Include(p => p.EstadoPedido)
                .Include(p => p.DetallePedidos)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        public async Task AgregarAsync(Pedido pedido)
        {
            await _ctx.Pedidos.AddAsync(pedido);
        }

        public Task<int> GuardarAsync()
        {
            return _ctx.SaveChangesAsync();
        }
    }
}
