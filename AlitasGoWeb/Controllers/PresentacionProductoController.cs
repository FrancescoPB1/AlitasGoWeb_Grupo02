using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    public class PresentacionProductoController : Controller
    {
        private readonly IPresentacionProductoService _presentacionService;
        private readonly IProductoService _productoService;
        private readonly ILogger<PresentacionProductoController> _logger;

        public PresentacionProductoController(
            IPresentacionProductoService presentacionService,
            IProductoService productoService,
            ILogger<PresentacionProductoController> logger)
        {
            _presentacionService = presentacionService;
            _productoService = productoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? idProducto)
        {
            if (idProducto is > 0)
            {
                var producto = await _productoService.ObtenerPorId(idProducto.Value);
                if (producto is null) return NotFound();

                ViewBag.Producto = producto;
                return View(await _presentacionService.ObtenerPorProducto(idProducto.Value));
            }

            return View(await _presentacionService.ObtenerTodos());
        }

        public async Task<IActionResult> Create(int? idProducto)
        {
            var modelo = new PresentacionProducto { Activo = true };

            if (idProducto is > 0)
            {
                var producto = await _productoService.ObtenerPorId(idProducto.Value);
                if (producto is null) return NotFound();

                modelo.IdProducto = producto.IdProducto;
                ViewBag.DesdeProducto = true;
                ViewBag.ProductoNombre = producto.Nombre;
            }

            await CargarCombos(modelo);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PresentacionProducto presentacion, bool desdeProducto = false)
        {
        
            if (!ModelState.IsValid)
                return await VolverAlFormulario(presentacion, desdeProducto, isEdit: false);

            var error = await _presentacionService.Registrar(presentacion);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return await VolverAlFormulario(presentacion, desdeProducto, isEdit: false);
            }

            TempData["SuccessMessage"] = "Presentación creada correctamente.";
            return RedirigirAlListado(presentacion.IdProducto, desdeProducto);
        }

        public async Task<IActionResult> Edit(int id, bool desdeProducto = false)
        {
            var presentacion = await _presentacionService.ObtenerPorId(id);
            if (presentacion is null) return NotFound();

            return await VolverAlFormulario(presentacion, desdeProducto, isEdit: true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PresentacionProducto presentacion, bool desdeProducto = false)
        {
            if (id != presentacion.IdPresentacion) return BadRequest();

            if (!ModelState.IsValid)
                return await VolverAlFormulario(presentacion, desdeProducto, isEdit: true);

            var error = await _presentacionService.Actualizar(presentacion);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return await VolverAlFormulario(presentacion, desdeProducto, isEdit: true);
            }

            TempData["SuccessMessage"] = "Presentación actualizada correctamente.";
            return RedirigirAlListado(presentacion.IdProducto, desdeProducto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool desdeProducto = false)
        {
            var presentacion = await _presentacionService.ObtenerPorId(id);
            if (presentacion is null) return NotFound();

            var error = await _presentacionService.Eliminar(id);
            if (error != null)
            {
                _logger.LogWarning("No se pudo eliminar la presentación {Id}: {Error}", id, error);
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["SuccessMessage"] = "Presentación eliminada.";
            }

            return RedirigirAlListado(presentacion.IdProducto, desdeProducto);
        }


        private IActionResult RedirigirAlListado(int idProducto, bool desdeProducto)
        {
            return desdeProducto
                ? RedirectToAction(nameof(Index), new { idProducto })
                : RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> VolverAlFormulario(PresentacionProducto presentacion, bool desdeProducto, bool isEdit)
        {
            await CargarCombos(presentacion);
            ViewBag.IsEdit = isEdit;
            ViewBag.DesdeProducto = desdeProducto;

            if (desdeProducto)
            {
                var producto = await _productoService.ObtenerPorId(presentacion.IdProducto);
                ViewBag.ProductoNombre = producto?.Nombre;
            }

            return View("Create", presentacion);
        }

        private async Task CargarCombos(PresentacionProducto presentacion)
        {
            var productos = (await _productoService.ObtenerTodos()).Where(p => p.Activo || p.IdProducto == presentacion.IdProducto); // activos + el actual
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre", presentacion.IdProducto);
        }
    }
}
