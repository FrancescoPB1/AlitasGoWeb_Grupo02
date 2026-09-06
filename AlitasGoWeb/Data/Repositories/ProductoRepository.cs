using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlitasGoWeb.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AlitasGoDbContext _context;

        public ProductoRepository(AlitasGoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodos()
        {
            return await _context.Productos
                .Include(p => p.CategoriaProducto)
                .Include(p => p.TipoProducto)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _context.Productos
                .Include(p => p.CategoriaProducto)
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task Agregar(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var producto = await ObtenerPorId(id);

            if (producto != null)
            {
                producto.Activo = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}