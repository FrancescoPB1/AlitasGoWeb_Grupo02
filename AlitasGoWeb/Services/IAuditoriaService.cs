using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface IAuditoriaService
    {
        // Agrega el registro; se confirma junto con la operación que audita (misma transacción).
        Task RegistrarAsync(string usuario, string accion, string entidad, string? idEntidad, string? detalle);

        // Para eventos que no acompañan a otra operación (p. ej. acceso denegado).
        Task RegistrarYGuardarAsync(string usuario, string accion, string entidad, string? idEntidad, string? detalle);

        Task<List<Auditoria>> ListarAsync(DateTime desde, DateTime hasta);
    }
}
