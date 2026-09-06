using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface ICategoriaProductoRepository
    {
        public Task<IEnumerable<CategoriaProducto>> ObtenerTodos();
        public Task<CategoriaProducto?> ObtenerPorId(int id);
        public Task Agregar(CategoriaProducto categoria);
        public Task Actualizar(CategoriaProducto categoria);
        public Task Eliminar(int id);
    }
}
