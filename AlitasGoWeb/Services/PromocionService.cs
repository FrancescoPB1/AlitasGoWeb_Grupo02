using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public class PromocionService : IPromocionService
    {
        private readonly IPromocionRepository _repo;
        private readonly IAuditoriaService _auditoria;
        private readonly IUnidadDeTrabajo _uow;

        public PromocionService(IPromocionRepository repo, IAuditoriaService auditoria, IUnidadDeTrabajo uow)
        {
            _repo = repo;
            _auditoria = auditoria;
            _uow = uow;
        }

        // Tabla Dias: 1 = Lunes ... 7 = Domingo.
        // Antes se comparaba el nombre del día generado por la cultura es-PE ("miércoles", "sábado",
        // con tilde) contra "Miercoles"/"Sabado" de la tabla: esos días nunca aplicaba promoción.
        public static int IdDiaDe(DateTime fecha) =>
            fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;

        public Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanal, DateTime fecha) =>
            _repo.ObtenerCodigoAplicableAsync(idProducto, idCanal, IdDiaDe(fecha));

        public Task<List<Promocion>> ListarAsync() => _repo.ListarAsync();
        public Task<List<TipoPromocion>> ListarTiposAsync() => _repo.ListarTiposAsync();
        public Task<List<CanalPromocion>> ListarCanalesAsync() => _repo.ListarCanalesAsync();
        public Task<List<Dia>> ListarDiasAsync() => _repo.ListarDiasAsync();

        public async Task<Resultado> CrearAsync(SolicitudPromocion s, string usuario)
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(s.Nombre)) errores.Add("El nombre de la promoción es obligatorio.");
            if ((await _repo.ListarTiposAsync()).All(t => t.IdTipoPromocion != s.IdTipoPromocion)) errores.Add("Selecciona un tipo de promoción válido.");
            if ((await _repo.ListarCanalesAsync()).All(c => c.IdCanalPromocion != s.IdCanalPromocion)) errores.Add("Selecciona un canal válido.");
            if ((await _repo.ListarDiasAsync()).All(d => d.IdDia != s.IdDia)) errores.Add("Selecciona un día válido.");

            var productos = await _repo.ObtenerProductosAsync(s.IdsProducto);
            if (productos.Count == 0) errores.Add("Selecciona al menos un producto.");
            if (productos.Any(p => p.EsCombo)) errores.Add("Los combos ya tienen precio propio: no se les aplica promoción.");
            if (errores.Count > 0) return Resultado.Error(errores.ToArray());

            var promocion = new Promocion
            {
                Nombre = s.Nombre.Trim(),
                IdTipoPromocion = s.IdTipoPromocion,
                IdCanalPromocion = s.IdCanalPromocion,
                IdDia = s.IdDia,
                Activo = true,
                Productos = productos
            };

            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                await _repo.AgregarAsync(promocion);
                await _auditoria.RegistrarAsync(usuario, "Crear promoción", "Promocion", null,
                    $"{promocion.Nombre} · día {s.IdDia} · productos {string.Join(",", productos.Select(p => p.IdProducto))}");
            });
            return Resultado.Ok("Promoción creada.");
        }

        public async Task<Resultado> AlternarActivoAsync(int idPromocion, string usuario)
        {
            var promocion = await _repo.ObtenerAsync(idPromocion);
            if (promocion == null) return Resultado.Error("La promoción no existe.");

            promocion.Activo = !promocion.Activo;
            await _auditoria.RegistrarAsync(usuario, promocion.Activo ? "Activar promoción" : "Desactivar promoción",
                "Promocion", idPromocion.ToString(), promocion.Nombre);
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok(promocion.Activo ? "Promoción activada." : "Promoción desactivada.");
        }
    }
}
