using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class CategoriaProductoController : ControladorBase
    {
        private readonly ICatalogoService _catalogo;

        public CategoriaProductoController(ICatalogoService catalogo)
        {
            _catalogo = catalogo;
        }

        public async Task<IActionResult> Index() => View(await _catalogo.ListarCategoriasAsync(false));

        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            return View(new CategoriaProducto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre")] CategoriaProducto categoria)
        {
            ViewBag.IsEdit = false;
            if (!ModelState.IsValid) return View(categoria);

            var r = await _catalogo.CrearCategoriaAsync(categoria, UsuarioActual);
            if (!r.Exito) { CopiarErrores(r, ModelState); return View(categoria); }

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _catalogo.ObtenerCategoriaAsync(id);
            if (categoria == null) return NotFound();
            ViewBag.IsEdit = true;
            return View("Create", categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaProducto,Nombre,Activo")] CategoriaProducto categoria)
        {
            if (id != categoria.IdCategoriaProducto) return NotFound();
            ViewBag.IsEdit = true;
            if (!ModelState.IsValid) return View("Create", categoria);

            var r = await _catalogo.ActualizarCategoriaAsync(categoria, UsuarioActual);
            if (!r.Exito) { CopiarErrores(r, ModelState); return View("Create", categoria); }

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        // POST desde el modal de confirmación (_ConfirmarEliminarModal): borrado lógico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Notificar(await _catalogo.DesactivarCategoriaAsync(id, UsuarioActual));
            return RedirectToAction(nameof(Index));
        }

        // Validación remota: ¿el nombre ya existe?
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> NombreDisponible(string nombre, int idCategoriaProducto) =>
            await _catalogo.NombreCategoriaDisponibleAsync(nombre, idCategoriaProducto)
                ? Json(true)
                : Json($"La categoría {nombre} ya existe.");
    }
}
