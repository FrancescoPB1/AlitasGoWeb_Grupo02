using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IProductoRepository
    {
        public Task<IEnumerable<Producto>> ObtenerTodos();
        public Task<Producto?> ObtenerPorId(int id);
        public Task Agregar(Producto producto);
        public Task Actualizar(Producto producto);
        public Task Eliminar(int id);
    }
}
