using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public class PresentacionProductoService 
    {
        private readonly IPresentacionProductoRepository _repositorio;

        public PresentacionProductoService(IPresentacionProductoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        //public Task<IEnumerable<PresentacionProducto>> ObtenerTodos()
        //    => _repositorio.ObtenerTodosAsync();

        //public async Task<IEnumerable<PresentacionProducto>> ObtenerPorProducto(int idProducto)
        //{
        //    if (idProducto <= 0) return Enumerable.Empty<PresentacionProducto>();
        //    return await _repositorio.ObtenerPorProductoIdAsync(idProducto);
        //}

        //public Task<PresentacionProducto?> ObtenerPorId(int id)
        //    => _repositorio.ObtenerPorIdAsync(id);

        //public async Task<string?> Registrar(PresentacionProducto presentacion)
        //{
        //    var error = Validar(presentacion);
        //    if (error != null) return error;

        //    bool creado = await _repositorio.CrearAsync(presentacion);
        //    return creado ? null : "No se pudo registrar la presentación.";
        //}

        //public async Task<string?> Actualizar(PresentacionProducto presentacion)
        //{
        //    var error = Validar(presentacion);
        //    if (error != null) return error;

        //    bool actualizado = await _repositorio.ActualizarAsync(presentacion);
        //    return actualizado ? null : "No se pudo actualizar la presentación.";
        //}

        //public async Task<string?> Eliminar(int id)
        //{
        //    bool eliminado = await _repositorio.EliminarAsync(id);
        //    return eliminado ? null : "La presentación no existe o no se pudo eliminar.";
        //}

        //private static string? Validar(PresentacionProducto p)
        //{
        //    if (p.IdProducto <= 0) return "Debe seleccionar un producto.";
        //    if (string.IsNullOrWhiteSpace(p.NombrePresentacion)) return "El nombre de la presentación es obligatorio.";
        //    if (p.CantidadUnidad <= 0) return "La cantidad debe ser mayor a cero.";
        //    if (p.Precio <= 0) return "El precio debe ser mayor a cero.";
        //    return null;
        //}
    }
}
