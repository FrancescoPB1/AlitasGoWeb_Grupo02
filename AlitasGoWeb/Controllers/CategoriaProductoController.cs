using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    public class CategoriaProductoController : Controller
    {
        private readonly ICategoriaProductoService _categoriaService;

        public CategoriaProductoController(ICategoriaProductoService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.ObtenerTodos();
            return View(categorias);
        }

        public IActionResult Create()
        {
            return View(new CategoriaProducto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProducto categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            var error = await _categoriaService.Registrar(categoria);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(categoria);
            }

            TempData["SuccessMessage"] = "Categoría registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _categoriaService.ObtenerPorId(id);
            if (categoria is null) return NotFound();

            ViewBag.IsEdit = true;
            return View("Create", categoria); // una sola vista para crear y editar
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaProducto categoria)
        {
            if (id != categoria.IdCategoriaProducto) return BadRequest();

            ViewBag.IsEdit = true;

            if (!ModelState.IsValid)
                return View("Create", categoria);

            var error = await _categoriaService.Actualizar(categoria);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View("Create", categoria);
            }

            TempData["SuccessMessage"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _categoriaService.Eliminar(id);
            if (error != null)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Categoría eliminada.";

            return RedirectToAction(nameof(Index));
        }
    }
}
