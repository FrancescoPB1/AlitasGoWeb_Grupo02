using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    // Reservado a Rosa. Un Cocinero que escriba /Reportes en la barra de direcciones recibe acceso denegado.
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class ReportesController : ControladorBase
    {
        private readonly IReporteService _reportes;

        public ReportesController(IReporteService reportes)
        {
            _reportes = reportes;
        }

        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta)
        {
            var d = (desde ?? DateTime.Today).Date;
            var h = (hasta ?? DateTime.Today).Date;
            var reporte = await _reportes.GenerarAsync(d, h);
            return View(new RangoFechasVM<ReporteVentas> { Desde = reporte.Desde, Hasta = reporte.Hasta, Datos = reporte });
        }
    }
}
