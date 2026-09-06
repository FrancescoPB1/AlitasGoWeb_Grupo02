using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class PresentacionProductoRepository : IPresentacionProductoRepository
    {
        private readonly AlitasGoDbContext _context;

        public PresentacionProductoRepository(AlitasGoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PresentacionProducto>> ObtenerTodosAsync()
        {
            return await _context.PresentacionProductos
                .Include(p => p.Producto)
                .OrderBy(p => p.Producto!.Nombre).ThenBy(p => p.Precio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<PresentacionProducto>> ObtenerPorProductoIdAsync(int idProducto)
        {
            return await _context.PresentacionProductos
                .Where(p => p.IdProducto == idProducto)
                .OrderBy(p => p.Precio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PresentacionProducto?> ObtenerPorIdAsync(int id)
        {
            return await _context.PresentacionProductos
                .Include(p => p.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPresentacion == id);
        }

        public async Task<bool> CrearAsync(PresentacionProducto presentacion)
        {
            _context.PresentacionProductos.Add(presentacion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActualizarAsync(PresentacionProducto presentacion)
        {
            presentacion.Producto = null; 
            _context.PresentacionProductos.Update(presentacion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var item = await _context.PresentacionProductos.FindAsync(id);
            if (item == null) return false;

            _context.PresentacionProductos.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
