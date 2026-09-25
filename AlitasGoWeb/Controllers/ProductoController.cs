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
        private readonly IComboService _combos;

        public ProductoController(ICatalogoService catalogo, IInventarioService inventario, IAltaProductoService alta, IComboService combos)
        {
            _catalogo = catalogo;
            _inventario = inventario;
            _alta = alta;
            _combos = combos;
        }

        public async Task<IActionResult> Index() => View(await _catalogo.ListarProductosAsync(false));

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync(false);
            await CargarIngredientesAsync(new List<IngredienteVM> { new() });
            await CargarComponentesAsync(new List<ComponenteVM> { new() });
            return View(new Producto { Activo = true });
        }

        // El producto se crea junto con lo que lo compone, en una sola transacción:
        // sus ingredientes (receta) o, si es un combo, sus productos.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(CamposProducto)] Producto producto, List<IngredienteVM> ingredientes, List<ComponenteVM> componentes)
        {
            if (ModelState.IsValid)
            {
                Resultado r;
                if (producto.EsCombo)
                {
                    r = await _alta.CrearComboAsync(producto, componentes.Select(c => new LineaComponente
                    {
                        IdProducto = c.IdProducto,
                        IdSabor = c.IdSabor,
                        Cantidad = c.Cantidad
                    }).ToList(), UsuarioActual);
                }
                else
                {
                    r = await _alta.CrearConRecetaAsync(producto, ingredientes.Select(i => new LineaIngrediente
                    {
                        IdInsumo = i.IdInsumo,
                        IdSabor = i.IdSabor,
                        Cantidad = i.Cantidad
                    }).ToList(), UsuarioActual);
                }
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            await CargarListasAsync(false);
            await CargarIngredientesAsync(ingredientes);
            await CargarComponentesAsync(componentes);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _catalogo.ObtenerProductoAsync(id);
            if (producto == null) return NotFound();
            await CargarListasAsync(true);
            ViewBag.RecetaActual = await _inventario.ListarRecetaAsync(id);
            ViewBag.ComponentesActuales = await _combos.ListarComponentesAsync(id);
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
            ViewBag.ComponentesActuales = await _combos.ListarComponentesAsync(id);
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

        // ---------- Combo: qué productos lo forman ----------
        public async Task<IActionResult> Componentes(int id)
        {
            var combo = await _catalogo.ObtenerProductoAsync(id);
            if (combo == null) return NotFound();
            if (!combo.EsCombo) return RedirectToAction(nameof(Receta), new { id });
            return View(await ArmarComponentesAsync(combo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarComponente(int id, ComponentesComboVM vm)
        {
            Notificar(await _combos.AgregarComponenteAsync(id, new LineaComponente
            {
                IdProducto = vm.Nuevo.IdProducto,
                IdSabor = vm.Nuevo.IdSabor,
                Cantidad = vm.Nuevo.Cantidad
            }, UsuarioActual));
            return RedirectToAction(nameof(Componentes), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarComponente(int id, int idComponente)
        {
            Notificar(await _combos.QuitarComponenteAsync(id, idComponente, UsuarioActual));
            return RedirectToAction(nameof(Componentes), new { id });
        }

        private async Task<ComponentesComboVM> ArmarComponentesAsync(Producto combo) => new()
        {
            Combo = combo,
            Lineas = await _combos.ListarComponentesAsync(combo.IdProducto),
            Productos = await ProductosParaComboAsync(),
            Sabores = (await _catalogo.ListarSaboresAsync())
                .Select(s => new SelectListItem(s.Nombre, s.IdSabor.ToString()))
        };

        // Un combo se arma con productos activos que no sean otros combos.
        private async Task<List<Producto>> ProductosParaComboAsync() =>
            (await _catalogo.ListarProductosAsync(true)).Where(p => !p.EsCombo).OrderBy(p => p.Nombre).ToList();

        // Listas para la sección «Productos del combo» del formulario de nuevo producto.
        private async Task CargarComponentesAsync(List<ComponenteVM> componentes)
        {
            if (componentes.Count == 0) componentes.Add(new ComponenteVM());
            ViewBag.Componentes = componentes;
            ViewBag.ProductosCombo = await ProductosParaComboAsync();
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
