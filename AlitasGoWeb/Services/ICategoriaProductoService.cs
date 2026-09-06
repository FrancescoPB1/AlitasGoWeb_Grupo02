using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface ICategoriaProductoService
    {
        public Task<IEnumerable<CategoriaProducto>> ObtenerTodos();
        public Task<CategoriaProducto?> ObtenerPorId(int id);
        public Task<string?> Registrar(CategoriaProducto categoria);
        public Task<string?> Actualizar(CategoriaProducto categoria);
        public Task<string?> Eliminar(int id);
    }
}
