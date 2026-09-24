using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    // "Que las cuentas hablen por sí solas": quién anuló, quién tocó un precio, quién intentó entrar.
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class AuditoriaController : ControladorBase
    {
        private readonly IAuditoriaService _auditoria;

        public AuditoriaController(IAuditoriaService auditoria)
        {
            _auditoria = auditoria;
        }

        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta)
        {
            var d = (desde ?? DateTime.Today.AddDays(-7)).Date;
            var h = (hasta ?? DateTime.Today).Date;
            var registros = await _auditoria.ListarAsync(d, h);
            return View(new RangoFechasVM<List<Auditoria>> { Desde = d, Hasta = h, Datos = registros });
        }
    }
}
