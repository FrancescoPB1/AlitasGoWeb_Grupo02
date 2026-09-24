using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    // Rosa puede ver en qué punto va cada entrega sin llamar a Brayan.
    [Authorize(Policy = Politicas.Reparto)]
    public class RepartoController : ControladorBase
    {
        private readonly IPedidoService _pedidos;
        private readonly ICatalogoService _catalogo;

        public RepartoController(IPedidoService pedidos, ICatalogoService catalogo)
        {
            _pedidos = pedidos;
            _catalogo = catalogo;
        }

        public async Task<IActionResult> Index() => View(await _pedidos.ListarRepartoAsync());

        // GET: /Reparto/Tablero — bloque que se refresca en tiempo real
        public async Task<IActionResult> Tablero() => PartialView("_Tablero", await _pedidos.ListarRepartoAsync());

        // POST: /Reparto/Despachar/5 (Listo → En reparto)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Despachar(int id)
        {
            Notificar(await _pedidos.CambiarEstadoAsync(id, Estados.EnReparto, UsuarioActual));
            return RedirectToAction(nameof(Index));
        }

        // GET: /Reparto/Entregar/5 — cobro contra entrega
        public async Task<IActionResult> Entregar(int id)
        {
            var pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            var vm = new CobroVM { IdPedido = id, Pedido = pedido, Controlador = "Reparto", Accion = "Entregar" };
            await CargarTiposPagoAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entregar(int id, CobroVM vm)
        {
            vm.IdPedido = id;
            vm.Controlador = "Reparto";
            vm.Accion = "Entregar";
            if (ModelState.IsValid)
            {
                var r = await _pedidos.CobrarAsync(id, PedidosController.ASolicitudCobro(vm), UsuarioActual);
                if (r.Exito)
                {
                    Notificar(r);
                    return RedirectToAction(nameof(Index));
                }
                CopiarErrores(r, ModelState);
            }
            vm.Pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (vm.Pedido == null) return NotFound();
            await CargarTiposPagoAsync(vm);
            return View(vm);
        }

        private async Task CargarTiposPagoAsync(CobroVM vm)
        {
            vm.TiposPago = (await _catalogo.ListarTiposPagoAsync())
                .Select(t => new SelectListItem(t.Nombre, t.IdTipoPago.ToString()));
        }
    }
}
