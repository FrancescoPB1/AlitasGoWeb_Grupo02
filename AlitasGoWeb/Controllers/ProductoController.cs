using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaProductoService _categoriaService;
        private readonly ITipoProductoService _tipoService;

        public ProductoController(IProductoService productoService,
            ICategoriaProductoService categoriaService, ITipoProductoService tipoService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _tipoService = tipoService;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerTodos();
            return View(productos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new Producto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(producto);
                return View(producto);
            }

            var error = await _productoService.Registrar(producto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                await CargarCombos(producto);
                return View(producto);
            }

            TempData["SuccessMessage"] = "Producto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _productoService.ObtenerPorId(id);
            if (producto is null) return NotFound();

            await CargarCombos(producto);
            ViewBag.IsEdit = true;
            return View("Create", producto); // una sola vista para crear y editar
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto) return BadRequest();

            ViewBag.IsEdit = true;

            if (!ModelState.IsValid)
            {
                await CargarCombos(producto);
                return View("Create", producto);
            }

            var error = await _productoService.Actualizar(producto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                await CargarCombos(producto);
                return View("Create", producto);
            }

            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _productoService.Eliminar(id);
            if (error != null)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Producto eliminado.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(Producto? producto = null)
        {
            var categorias = (await _categoriaService.ObtenerTodos()).Where(c => c.Activo || c.IdCategoriaProducto == producto?.IdCategoriaProducto); // activas + la actual
            var tipos = await _tipoService.ObtenerTodos();

            ViewBag.CategoriaProductos = new SelectList(categorias, "IdCategoriaProducto", "Nombre", producto?.IdCategoriaProducto);
            ViewBag.TipoProductos = new SelectList(tipos, "IdTipoProducto", "Nombre", producto?.IdTipoProducto);
        }
    }
}
