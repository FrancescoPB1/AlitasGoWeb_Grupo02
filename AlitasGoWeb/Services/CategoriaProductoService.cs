using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public class CategoriaProductoService: ICategoriaProductoService
    {
        private readonly ICategoriaProductoRepository _repository;
        public CategoriaProductoService(ICategoriaProductoRepository repository) => _repository = repository;

        public async Task<IEnumerable<CategoriaProducto>> ObtenerTodos() => await _repository.ObtenerTodos();

        public async Task<CategoriaProducto?> ObtenerPorId(int id) => await _repository.ObtenerPorId(id);

        public async Task<string?> Registrar(CategoriaProducto categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return "El nombre es obligatorio.";

            categoria.Activo = true;
            await _repository.Agregar(categoria);
            return null;
        }

        public async Task<string?> Actualizar(CategoriaProducto categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return "El nombre es obligatorio.";

            await _repository.Actualizar(categoria);
            return null;
        }

        public async Task<string?> Eliminar(int id)
        {
            var categoria = await _repository.ObtenerPorId(id);
            if (categoria == null)
                return "La categoría no existe.";

            await _repository.Eliminar(id);
            return null;
        }
    }
}
