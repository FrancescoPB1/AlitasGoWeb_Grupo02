using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public class TipoProductoService:ITipoProductoService
    {
        private readonly ITipoProductoRepository _repository;
        public TipoProductoService(ITipoProductoRepository repository) => _repository = repository;

        public async Task<IEnumerable<TipoProducto>> ObtenerTodos() => await _repository.ObtenerTodos();
    }
}
