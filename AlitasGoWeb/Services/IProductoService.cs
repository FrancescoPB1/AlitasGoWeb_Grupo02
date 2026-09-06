using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface IProductoService
    {
        public Task<IEnumerable<Producto>> ObtenerTodos();
        public Task<Producto?> ObtenerPorId(int id);
        public Task<string?> Registrar(Producto producto);
        public Task<string?> Actualizar(Producto producto);
        public Task<string?> Eliminar(int id);
    }
}
