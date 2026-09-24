using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    // Pantalla de cocina: alto contraste, lo esencial en grande y aviso por voz.
    [Authorize(Policy = Politicas.Cocina)]
    public class CocinaController : ControladorBase
    {
        private readonly IPedidoService _pedidos;

        public CocinaController(IPedidoService pedidos)
        {
            _pedidos = pedidos;
        }

        public async Task<IActionResult> Index() => View(await _pedidos.ListarCocinaAsync());

        // GET: /Cocina/Tablero — la pantalla lo consulta cada pocos segundos sin recargar la página.
        public async Task<IActionResult> Tablero() => PartialView("_Tablero", await _pedidos.ListarCocinaAsync());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Iniciar(int id) => CambiarAsync(id, Estados.EnPreparacion);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> MarcarListo(int id) => CambiarAsync(id, Estados.Listo);

        private async Task<IActionResult> CambiarAsync(int id, int nuevoEstado)
        {
            var r = await _pedidos.CambiarEstadoAsync(id, nuevoEstado, UsuarioActual);
            var mensaje = r.Exito ? r.Mensaje : string.Join(" ", r.Errores);

            if (EsAjax) return Json(new { ok = r.Exito, mensaje });

            Notificar(r);
            return RedirectToAction(nameof(Index));
        }
    }
}
