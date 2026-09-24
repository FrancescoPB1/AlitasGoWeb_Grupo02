using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    // Rosa inventa promociones según la temporada sin tocar código:
    // elige el tipo (estrategia), el día, el canal y los productos.
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class PromocionesController : ControladorBase
    {
        private readonly IPromocionService _promociones;
        private readonly ICatalogoService _catalogo;

        public PromocionesController(IPromocionService promociones, ICatalogoService catalogo)
        {
            _promociones = promociones;
            _catalogo = catalogo;
        }

        public async Task<IActionResult> Index() => View(await _promociones.ListarAsync());

        public async Task<IActionResult> Create()
        {
            var vm = new PromocionFormVM();
            await CargarListasAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromocionFormVM vm)
        {
            if (ModelState.IsValid)
            {
                var r = await _promociones.CrearAsync(new SolicitudPromocion
                {
                    Nombre = vm.Nombre,
                    IdTipoPromocion = vm.IdTipoPromocion,
                    IdCanalPromocion = vm.IdCanalPromocion,
                    IdDia = vm.IdDia,
                    IdsProducto = vm.IdsProducto
                }, UsuarioActual);
                if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
                CopiarErrores(r, ModelState);
            }
            await CargarListasAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Alternar(int id)
        {
            Notificar(await _promociones.AlternarActivoAsync(id, UsuarioActual));
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync(PromocionFormVM vm)
        {
            vm.Tipos = (await _promociones.ListarTiposAsync())
                .Where(t => t.Codigo != "NORMAL")
                .Select(t => new SelectListItem($"{t.Nombre} ({t.Codigo})", t.IdTipoPromocion.ToString()));
            vm.CanalesPromo = (await _promociones.ListarCanalesAsync())
                .Select(c => new SelectListItem(c.Nombre, c.IdCanalPromocion.ToString()));
            vm.Dias = (await _promociones.ListarDiasAsync())
                .Select(d => new SelectListItem(d.Nombre, d.IdDia.ToString()));
            vm.Productos = (await _catalogo.ListarProductosParaPedidoAsync())
                .Where(p => !p.EsCombo)
                .Select(p => new SelectListItem($"{p.Nombre} - S/ {p.Precio:0.00}", p.IdProducto.ToString(), vm.IdsProducto.Contains(p.IdProducto)));
        }
    }
}
