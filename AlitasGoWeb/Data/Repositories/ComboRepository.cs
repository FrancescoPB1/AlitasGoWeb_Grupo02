using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class ComboRepository : IComboRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public ComboRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<List<ComboProducto>> ListarComponentesAsync(int idCombo) =>
            _ctx.ComboProductos.AsNoTracking()
                .Include(c => c.ProductoComponente)
                .Include(c => c.Sabor)
                .Where(c => c.IdProductoCombo == idCombo)
                .OrderBy(c => c.ProductoComponente!.Nombre)
                .ToListAsync();

        public Task<ComboProducto?> ObtenerAsync(int idCombo, int idComponente) =>
            _ctx.ComboProductos.FirstOrDefaultAsync(c => c.IdProductoCombo == idCombo && c.IdProductoComponente == idComponente);

        public async Task AgregarAsync(ComboProducto componente) => await _ctx.ComboProductos.AddAsync(componente);

        public void Eliminar(ComboProducto componente) => _ctx.ComboProductos.Remove(componente);
    }
}
