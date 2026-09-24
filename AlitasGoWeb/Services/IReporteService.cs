using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public interface IReporteService
    {
        Task<ReporteVentas> GenerarAsync(DateTime desde, DateTime hasta);
    }
}
