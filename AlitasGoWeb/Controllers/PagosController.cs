using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlitasGoWeb.Controllers
{
    // Historial de pagos: datos de dinero de todo el negocio → solo el Administrador.
    [Authorize(Policy = Politicas.SoloAdmin)]
    public class PagosController : ControladorBase
    {
        private readonly IPagoService _pagos;
        private readonly ICatalogoService _catalogo;

        public PagosController(IPagoService pagos, ICatalogoService catalogo)
        {
            _pagos = pagos;
            _catalogo = catalogo;
        }

        // GET: /Pagos  (por defecto, los últimos 7 días)
        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta, int? idTipoPago, string? tipoComprobante)
        {
            var vm = new HistorialPagosVM
            {
                Desde = (desde ?? DateTime.Today.AddDays(-7)).Date,
                Hasta = (hasta ?? DateTime.Today).Date,
                IdTipoPago = idTipoPago,
                TipoComprobante = tipoComprobante
            };

            vm.Pagos = await _pagos.ListarAsync(vm.Desde, vm.Hasta, idTipoPago, tipoComprobante);
            vm.TiposPago = (await _catalogo.ListarTiposPagoAsync())
                .Select(t => new SelectListItem(t.Nombre, t.IdTipoPago.ToString(), t.IdTipoPago == idTipoPago));

            return View(vm);
        }

        // GET: /Pagos/Details/5   (5 = número del pedido pagado)
        public async Task<IActionResult> Details(int id)
        {
            var pago = await _pagos.ObtenerAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }
    }
}
