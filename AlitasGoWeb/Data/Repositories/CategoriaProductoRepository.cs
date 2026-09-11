using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class CategoriaProductoRepository
    {
        private readonly AlitasGoDbContext _context;
        public CategoriaProductoRepository(AlitasGoDbContext context) => _context = context;

        //public async Task<IEnumerable<CategoriaProducto>> ObtenerTodos()
        //{
        //    return await _context.CategoriaProductos.ToListAsync();
        //}

        //public async Task<CategoriaProducto?> ObtenerPorId(int id)
        //{
        //    return await _context.CategoriaProductos.FindAsync(id);
        //}

        //public async Task Agregar(CategoriaProducto categoria)
        //{
        //    _context.CategoriaProductos.Add(categoria);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task Actualizar(CategoriaProducto categoria)
        //{
        //    _context.CategoriaProductos.Update(categoria);
        //    await _context.SaveChangesAsync();
        //}

        public async Task Eliminar(int id)
        {
            //var categoria = await ObtenerPorId(id);
            //if (categoria != null)
            //{
            //    categoria.Activo = false;
            //    await _context.SaveChangesAsync();
            //}
        }
    }
}
