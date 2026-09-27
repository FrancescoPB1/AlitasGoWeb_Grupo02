using AlitasGoWeb.Models;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    [Authorize(Policy = Politicas.TomarPedidos)]
    public class ClientesController : ControladorBase
    {
        private readonly ICatalogoService _catalogo;

        public ClientesController(ICatalogoService catalogo)
        {
            _catalogo = catalogo;
        }

        public async Task<IActionResult> Index() => View(await _catalogo.ListarClientesAsync());

        public IActionResult Create(bool volverAPedido = false, int? idNroMesa = null, int? canal = null)
        {
            ViewData["VolverAPedido"] = volverAPedido;
            ViewData["IdNroMesa"] = idNroMesa;   // para volver al pedido con la misma mesa y canal
            ViewData["Canal"] = canal;
            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,Nombre,ApellidoPaterno,ApellidoMaterno")] Cliente cliente, bool volverAPedido = false,
            int? idNroMesa = null, int? canal = null)
        {
            ViewData["VolverAPedido"] = volverAPedido;
            ViewData["IdNroMesa"] = idNroMesa;
            ViewData["Canal"] = canal;
            if (!ModelState.IsValid) return View(cliente);

            var r = await _catalogo.CrearClienteAsync(cliente);
            if (!r.Exito)
            {
                CopiarErrores(r, ModelState);
                return View(cliente);
            }

            Notificar(r);
            return volverAPedido
                ? RedirectToAction("Create", "Pedidos", new { idCliente = r.Valor, idNroMesa, canal })
                : RedirectToAction(nameof(Index));
        }
    }
}
