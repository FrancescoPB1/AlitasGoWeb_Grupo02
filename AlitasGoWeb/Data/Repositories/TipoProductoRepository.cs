using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class TipoProductoRepository: ITipoProductoRepository
    {
        private readonly AlitasGoDbContext _context;
        public TipoProductoRepository(AlitasGoDbContext context) => _context = context;

        public async Task<IEnumerable<TipoProducto>> ObtenerTodos()
        {
            return await _context.TipoProductos.Where(t => t.Activo).ToListAsync();
        }
    }
}
