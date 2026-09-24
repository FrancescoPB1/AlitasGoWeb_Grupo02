using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class UsuariosController : ControladorBase
    {
        private readonly IUsuarioService _usuarios;

        public UsuariosController(IUsuarioService usuarios)
        {
            _usuarios = usuarios;
        }

        public async Task<IActionResult> Index() => View(await _usuarios.ListarAsync());

        public IActionResult Create() => View(new UsuarioCreateVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var r = await _usuarios.CrearAsync(vm.Email, vm.Password, vm.Rol, UsuarioActual);
            if (!r.Exito) { CopiarErrores(r, ModelState); return View(vm); }

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(string id, string rol)
        {
            Notificar(await _usuarios.CambiarRolAsync(id, rol, UsuarioActual));
            return RedirectToAction(nameof(Index));
        }
    }
}
