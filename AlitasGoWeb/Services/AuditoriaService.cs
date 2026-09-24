using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _repo;
        private readonly IUnidadDeTrabajo _uow;
        private readonly IReloj _reloj;

        public AuditoriaService(IAuditoriaRepository repo, IUnidadDeTrabajo uow, IReloj reloj)
        {
            _repo = repo;
            _uow = uow;
            _reloj = reloj;
        }

        public Task RegistrarAsync(string usuario, string accion, string entidad, string? idEntidad, string? detalle)
        {
            return _repo.AgregarAsync(new Auditoria
            {
                Fecha = _reloj.Ahora,
                Usuario = Recortar(string.IsNullOrWhiteSpace(usuario) ? "(anónimo)" : usuario, 256)!,
                Accion = Recortar(accion, 100)!,
                Entidad = Recortar(entidad, 50)!,
                IdEntidad = Recortar(idEntidad, 50),
                Detalle = Recortar(detalle, 500)
            });
        }

        public async Task RegistrarYGuardarAsync(string usuario, string accion, string entidad, string? idEntidad, string? detalle)
        {
            await RegistrarAsync(usuario, accion, entidad, idEntidad, detalle);
            await _uow.GuardarCambiosAsync();
        }

        public Task<List<Auditoria>> ListarAsync(DateTime desde, DateTime hasta) =>
            _repo.ListarAsync(desde.Date, hasta.Date.AddDays(1), 500);

        private static string? Recortar(string? texto, int max) =>
            texto == null ? null : (texto.Length <= max ? texto : texto[..max]);
    }
}
