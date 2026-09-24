using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    [Authorize]
    public class CuentaController : ControladorBase
    {
        private readonly IAuditoriaService _auditoria;

        public CuentaController(IAuditoriaService auditoria)
        {
            _auditoria = auditoria;
        }

        // Ruta de acceso denegado: mensaje genérico + registro del intento para auditoría.
        public async Task<IActionResult> AccesoDenegado(string? returnUrl)
        {
            await _auditoria.RegistrarYGuardarAsync(UsuarioActual, "Acceso denegado", "Ruta", null, returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
