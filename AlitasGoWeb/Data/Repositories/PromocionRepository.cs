using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class PromocionRepository : IPromocionRepository
    {
        private const string CanalTodos = "Todos";
        private readonly AlitasGoDbContext _ctx;

        public PromocionRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanalAtencion, int idDia)
        {
            // Nombre del canal del pedido ("Salon" o "Delivery"), igual al de CanalesPromocion
            var nombreCanal = await _ctx.CanalesAtencion
                .Where(c => c.IdCanalAtencion == idCanalAtencion)
                .Select(c => c.Nombre)
                .FirstOrDefaultAsync();

            if (nombreCanal == null) return null;

            return await _ctx.Promociones
                .Where(p => p.Activo
                         && p.IdDia == idDia
                         && (p.CanalPromocion!.Nombre == CanalTodos || p.CanalPromocion.Nombre == nombreCanal)
                         && p.Productos.Any(pr => pr.IdProducto == idProducto))
                .OrderBy(p => p.IdPromocion)   // regla determinista: la más antigua primero
                .Select(p => p.TipoPromocion!.Codigo)
                .FirstOrDefaultAsync();
        }

        public Task<List<Promocion>> ListarAsync() =>
            _ctx.Promociones.AsNoTracking()
                .Include(p => p.TipoPromocion)
                .Include(p => p.CanalPromocion)
                .Include(p => p.Dia)
                .Include(p => p.Productos)
                .OrderBy(p => p.IdDia).ThenBy(p => p.Nombre)
                .ToListAsync();

        public Task<Promocion?> ObtenerAsync(int idPromocion) =>
            _ctx.Promociones.FirstOrDefaultAsync(p => p.IdPromocion == idPromocion);

        public async Task AgregarAsync(Promocion promocion) => await _ctx.Promociones.AddAsync(promocion);

        public Task<List<TipoPromocion>> ListarTiposAsync() =>
            _ctx.TiposPromocion.AsNoTracking().Where(t => t.Activo).OrderBy(t => t.IdTipoPromocion).ToListAsync();

        public Task<List<CanalPromocion>> ListarCanalesAsync() =>
            _ctx.CanalesPromocion.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.IdCanalPromocion).ToListAsync();

        public Task<List<Dia>> ListarDiasAsync() =>
            _ctx.Dias.AsNoTracking().Where(d => d.Activo).OrderBy(d => d.IdDia).ToListAsync();

        public Task<List<Producto>> ObtenerProductosAsync(IEnumerable<int> ids)
        {
            var lista = ids.Distinct().ToList();
            return _ctx.Productos.Where(p => lista.Contains(p.IdProducto)).ToListAsync();
        }
    }
}
