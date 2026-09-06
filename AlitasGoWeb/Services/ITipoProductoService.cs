using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface ITipoProductoService
    {
        public Task<IEnumerable<TipoProducto>> ObtenerTodos();
    }
}
