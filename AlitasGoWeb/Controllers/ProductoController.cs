using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
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
        private readonly IAltaProductoService _alta;

        public ProductoController(ICatalogoService catalogo, IInventarioService inventario, IAltaProductoService alta)
        {
            _catalogo = catalogo;
            _inventario = inventario;
            _alta = alta;
        }

        public async Task<IActionResult> Index() => View(await _catalogo.ListarProductosAsync(false));

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync(false);
            await CargarIngredientesAsync(new List<IngredienteVM> { new() });
            return View(new Producto { Activo = true });
        }

        // El producto y sus ingredientes (receta) se crean juntos, en una sola transacción.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(CamposProducto)] Producto producto, List<IngredienteVM> ingredientes)
        {
            if (ModelState.IsValid)
            {
                var lineas = ingredientes.Select(i => new LineaIngrediente
                {
                    IdInsumo = i.IdInsumo,
                    IdSabor = i.IdSabor,
                    Cantidad = i.Cantidad
                }).ToList();

                var r = await _alta.CrearConRecetaAsync(producto, lineas, UsuarioActual);
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            await CargarListasAsync(false);
            await CargarIngredientesAsync(ingredientes);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _catalogo.ObtenerProductoAsync(id);
            if (producto == null) return NotFound();
            await CargarListasAsync(true);
            ViewBag.RecetaActual = await _inventario.ListarRecetaAsync(id);
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
            ViewBag.RecetaActual = await _inventario.ListarRecetaAsync(id);
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

        // Listas para la sección «Ingredientes» del formulario de nuevo producto.
        private async Task CargarIngredientesAsync(List<IngredienteVM> ingredientes)
        {
            if (ingredientes.Count == 0) ingredientes.Add(new IngredienteVM());
            ViewBag.Ingredientes = ingredientes;
            ViewBag.Insumos = (await _inventario.ListarInsumosAsync())
                .Select(i => new SelectListItem($"{i.Nombre} ({i.UnidadMedida})", i.IdInsumo.ToString())).ToList();
            ViewBag.Sabores = (await _catalogo.ListarSaboresAsync())
                .Select(s => new SelectListItem(s.Nombre, s.IdSabor.ToString())).ToList();
        }

        private async Task CargarListasAsync(bool esEdicion)
        {
            ViewBag.IsEdit = esEdicion;
            ViewBag.CategoriaProductos = new SelectList(await _catalogo.ListarCategoriasAsync(true), "IdCategoriaProducto", "Nombre");
            ViewBag.TipoProductos = new SelectList(await _catalogo.ListarTiposProductoAsync(), "IdTipoProducto", "Nombre");
        }
    }
}
