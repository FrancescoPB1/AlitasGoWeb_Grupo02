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
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class InsumosController : ControladorBase
    {
        private readonly IInventarioService _inventario;

        public InsumosController(IInventarioService inventario)
        {
            _inventario = inventario;
        }

        // Los insumos bajo el mínimo aparecen primero y resaltados.
        public async Task<IActionResult> Index()
        {
            var insumos = await _inventario.ListarInsumosAsync();
            return View(insumos.OrderByDescending(i => i.BajoMinimo).ThenBy(i => i.Nombre).ToList());
        }

        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            return View(new Insumo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,UnidadMedida,StockActual,StockMinimo")] Insumo insumo)
        {
            ViewBag.IsEdit = false;
            if (!ModelState.IsValid) return View(insumo);

            var r = await _inventario.CrearInsumoAsync(insumo, UsuarioActual);
            if (!r.Exito) { CopiarErrores(r, ModelState); return View(insumo); }

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var insumo = await _inventario.ObtenerInsumoAsync(id);
            if (insumo == null) return NotFound();
            ViewBag.IsEdit = true;
            return View("Create", insumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInsumo,Nombre,UnidadMedida,StockMinimo")] Insumo insumo)
        {
            if (id != insumo.IdInsumo) return NotFound();
            ViewBag.IsEdit = true;
            if (!ModelState.IsValid) return View("Create", insumo);

            var r = await _inventario.EditarInsumoAsync(insumo, UsuarioActual);
            if (!r.Exito) { CopiarErrores(r, ModelState); return View("Create", insumo); }

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Insumos/Compra — cada compra es un movimiento de entrada
        public async Task<IActionResult> Compra(int? idInsumo)
        {
            var vm = new CompraVM();
            vm.Lineas.Add(new LineaCompraVM { IdInsumo = idInsumo ?? 0 });
            await CargarInsumosAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Compra(CompraVM vm)
        {
            if (ModelState.IsValid)
            {
                var lineas = vm.Lineas.Select(l => new LineaCompra
                {
                    IdInsumo = l.IdInsumo,
                    Cantidad = l.Cantidad,
                    CostoUnitario = l.CostoUnitario
                }).ToList();

                var r = await _inventario.RegistrarCompraAsync(lineas, UsuarioActual);
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            if (vm.Lineas.Count == 0) vm.Lineas.Add(new LineaCompraVM());
            await CargarInsumosAsync(vm);
            return View(vm);
        }

        public async Task<IActionResult> Movimientos(int? idInsumo)
        {
            ViewData["IdInsumo"] = idInsumo;
            ViewData["Insumos"] = (await _inventario.ListarInsumosAsync())
                .Select(i => new SelectListItem(i.Nombre, i.IdInsumo.ToString(), i.IdInsumo == idInsumo)).ToList();
            return View(await _inventario.ListarMovimientosAsync(idInsumo));
        }

        private async Task CargarInsumosAsync(CompraVM vm)
        {
            vm.Insumos = (await _inventario.ListarInsumosAsync())
                .Select(i => new SelectListItem($"{i.Nombre} ({i.UnidadMedida}) · stock {i.StockActual:0.##}", i.IdInsumo.ToString()));
        }
    }
}
