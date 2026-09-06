using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public class ProductoService: IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodos()
        {
            return await _repository.ObtenerTodos();
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _repository.ObtenerPorId(id);
        }

        public async Task<string?> Registrar(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return "El nombre es obligatorio.";

            if (producto.IdCategoriaProducto <= 0)
                return "Debe seleccionar una categoría.";

            if (producto.IdTipoProducto <= 0)
                return "Debe seleccionar un tipo de producto.";

            producto.Activo = true;

            await _repository.Agregar(producto);

            return null;
        }

        public async Task<string?> Actualizar(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return "El nombre es obligatorio.";

            await _repository.Actualizar(producto);

            return null;
        }

        public async Task<string?> Eliminar(int id)
        {
            var producto = await _repository.ObtenerPorId(id);

            if (producto == null)
                return "El producto no existe.";

            await _repository.Eliminar(id);

            return null;
        }
    }
}
