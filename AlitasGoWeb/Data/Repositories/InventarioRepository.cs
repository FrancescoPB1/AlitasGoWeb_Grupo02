using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public InventarioRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<List<Insumo>> ListarInsumosAsync() =>
            _ctx.Insumos.AsNoTracking().OrderBy(i => i.Nombre).ToListAsync();

        public Task<Insumo?> ObtenerInsumoAsync(int idInsumo) =>
            _ctx.Insumos.FirstOrDefaultAsync(i => i.IdInsumo == idInsumo);

        public Task<List<Insumo>> ObtenerInsumosAsync(IEnumerable<int> ids)
        {
            var lista = ids.Distinct().ToList();
            return _ctx.Insumos.Where(i => lista.Contains(i.IdInsumo)).ToListAsync();
        }

        public Task<bool> ExisteInsumoAsync(string nombre, int excluirId) =>
            _ctx.Insumos.AnyAsync(i => i.Nombre == nombre && i.IdInsumo != excluirId);

        public async Task AgregarInsumoAsync(Insumo insumo) => await _ctx.Insumos.AddAsync(insumo);

        public Task<List<Receta>> ObtenerRecetasAsync(IEnumerable<int> idsProducto)
        {
            var lista = idsProducto.Distinct().ToList();
            return _ctx.Recetas.AsNoTracking()
                .Where(r => r.Activo && lista.Contains(r.IdProducto))
                .ToListAsync();
        }

        public Task<List<Receta>> ListarRecetaDeProductoAsync(int idProducto) =>
            _ctx.Recetas.AsNoTracking()
                .Include(r => r.Insumo)
                .Include(r => r.Sabor)
                .Where(r => r.IdProducto == idProducto && r.Activo)
                .OrderBy(r => r.Insumo!.Nombre)
                .ToListAsync();

        public Task<Receta?> ObtenerRecetaAsync(int idReceta) =>
            _ctx.Recetas.FirstOrDefaultAsync(r => r.IdProductoInsumo == idReceta);

        public async Task AgregarRecetaAsync(Receta receta) => await _ctx.Recetas.AddAsync(receta);

        public void EliminarReceta(Receta receta) => _ctx.Recetas.Remove(receta);

        public Task<List<ComboProducto>> ObtenerComponentesAsync(IEnumerable<int> idsCombo)
        {
            var lista = idsCombo.Distinct().ToList();
            return _ctx.ComboProductos.AsNoTracking()
                .Where(c => lista.Contains(c.IdProductoCombo))
                .ToListAsync();
        }

        public async Task AgregarCompraAsync(CompraInsumos compra) => await _ctx.ComprasInsumos.AddAsync(compra);

        public async Task AgregarMovimientoAsync(MovimientoInventario movimiento) =>
            await _ctx.MovimientosInventario.AddAsync(movimiento);

        public Task<List<MovimientoInventario>> ListarMovimientosAsync(int? idInsumo, int maximo)
        {
            var q = _ctx.MovimientosInventario.AsNoTracking().Include(m => m.Insumo).AsQueryable();
            if (idInsumo.HasValue) q = q.Where(m => m.IdInsumo == idInsumo.Value);
            return q.OrderByDescending(m => m.Fecha).ThenByDescending(m => m.IdMovimientoInventario)
                    .Take(maximo).ToListAsync();
        }
    }
}
