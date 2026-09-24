using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    // Solo Rosa cambia precios (el cajero jamás): política SoloAdmin en el controlador.
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class ProductoController : ControladorBase
    {
        private const string CamposProducto = "IdProducto,Nombre,IdCategoriaProducto,IdTipoProducto,Descripcion,Precio,RequiereSabor,Activo";

        private readonly ICatalogoService _catalogo;
        private readonly IInventarioService _inventario;

        public ProductoController(ICatalogoService catalogo, IInventarioService inventario)
        {
            _catalogo = catalogo;
            _inventario = inventario;
        }

        public async Task<IActionResult> Index() => View(await _catalogo.ListarProductosAsync(false));

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync(false);
            return View(new Producto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(CamposProducto)] Producto producto)
        {
            if (ModelState.IsValid)
            {
                var r = await _catalogo.CrearProductoAsync(producto, UsuarioActual);
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            await CargarListasAsync(false);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _catalogo.ObtenerProductoAsync(id);
            if (producto == null) return NotFound();
            await CargarListasAsync(true);
            return View("Create", producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(CamposProducto)] Producto producto)
        {
            if (id != producto.IdProducto) return NotFound();
            if (ModelState.IsValid)
            {
                var r = await _catalogo.ActualizarProductoAsync(producto, UsuarioActual);
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            await CargarListasAsync(true);
            return View("Create", producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Notificar(await _catalogo.DesactivarProductoAsync(id, UsuarioActual));
            return RedirectToAction(nameof(Index));
        }

        // ---------- Receta: cuánto insumo consume cada producto ----------
        public async Task<IActionResult> Receta(int id)
        {
            var producto = await _catalogo.ObtenerProductoAsync(id);
            if (producto == null) return NotFound();
            return View(await ArmarRecetaAsync(producto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarReceta(int id, RecetaVM vm)
        {
            Notificar(await _inventario.AgregarLineaRecetaAsync(id, vm.IdInsumo, vm.IdSabor, vm.Cantidad, UsuarioActual));
            return RedirectToAction(nameof(Receta), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarReceta(int id, int idReceta)
        {
            Notificar(await _inventario.QuitarLineaRecetaAsync(idReceta, UsuarioActual));
            return RedirectToAction(nameof(Receta), new { id });
        }

        private async Task<RecetaVM> ArmarRecetaAsync(Producto producto) => new()
        {
            Producto = producto,
            Lineas = await _inventario.ListarRecetaAsync(producto.IdProducto),
            Insumos = (await _inventario.ListarInsumosAsync())
                .Select(i => new SelectListItem($"{i.Nombre} ({i.UnidadMedida})", i.IdInsumo.ToString())),
            Sabores = (await _catalogo.ListarSaboresAsync())
                .Select(s => new SelectListItem(s.Nombre, s.IdSabor.ToString()))
        };

        private async Task CargarListasAsync(bool esEdicion)
        {
            ViewBag.IsEdit = esEdicion;
            ViewBag.CategoriaProductos = new SelectList(await _catalogo.ListarCategoriasAsync(true), "IdCategoriaProducto", "Nombre");
            ViewBag.TipoProductos = new SelectList(await _catalogo.ListarTiposProductoAsync(), "IdTipoProducto", "Nombre");
        }
    }
}
