using AlitasGoWeb.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    [Authorize]
    public class InicioController : ControladorBase
    {
        // Cada rol aterriza en su pantalla de trabajo.
        public IActionResult Index()
        {
            if (User.IsInRole(RolesSistema.Administrador)) return RedirectToAction("Index", "Pedidos");
            if (User.IsInRole(RolesSistema.Cajero)) return RedirectToAction("Index", "Salon");
            if (User.IsInRole(RolesSistema.Cocinero)) return RedirectToAction("Index", "Cocina");
            if (User.IsInRole(RolesSistema.Repartidor)) return RedirectToAction("Index", "Reparto");
            return View("SinRol");
        }

        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}
