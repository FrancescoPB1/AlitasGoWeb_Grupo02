using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface ITipoProductoRepository
    {
        public Task<IEnumerable<TipoProducto>> ObtenerTodos();
       
    }
}
