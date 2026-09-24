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
    // Presentación: solo traduce HTTP; las reglas viven en PedidoService.
    [Authorize(Policy = Politicas.TomarPedidos)]
    public class PedidosController : ControladorBase
    {
        private readonly IPedidoService _pedidos;
        private readonly ICatalogoService _catalogo;
        private readonly ICalculadoraIgv _igv;
        private readonly IConfiguracionSistema _config;

        public PedidosController(IPedidoService pedidos, ICatalogoService catalogo, ICalculadoraIgv igv, IConfiguracionSistema config)
        {
            _pedidos = pedidos;
            _catalogo = catalogo;
            _igv = igv;
            _config = config;
        }

        // GET: /Pedidos?fecha=2026-09-23&idEstado=2
        public async Task<IActionResult> Index(DateTime? fecha, int? idEstado) =>
            View(await ArmarListaAsync(fecha, idEstado));

        // GET: /Pedidos/Lista — bloque que se refresca en tiempo real
        public async Task<IActionResult> Lista(DateTime? fecha, int? idEstado) =>
            PartialView("_Lista", await ArmarListaAsync(fecha, idEstado));

        private async Task<PedidosIndexVM> ArmarListaAsync(DateTime? fecha, int? idEstado)
        {
            var dia = (fecha ?? DateTime.Today).Date;
            return new PedidosIndexVM
            {
                Fecha = dia,
                IdEstado = idEstado,
                Pedidos = await _pedidos.ListarAsync(dia, idEstado),
                Estados = new[] { Estados.Recibido, Estados.EnPreparacion, Estados.Listo, Estados.EnReparto,
                                  Estados.Servido, Estados.Entregado, Estados.Anulado }
                    .Select(e => new SelectListItem(MaquinaEstadosPedido.Nombre(e), e.ToString(), e == idEstado))
            };
        }

        // GET: /Pedidos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        // GET: /Pedidos/Create?idNroMesa=3  ó  /Pedidos/Create?canal=2
        public async Task<IActionResult> Create(int? idNroMesa, int? canal, int? idCliente)
        {
            var vm = new PedidoCreateVM
            {
                IdNroMesa = idNroMesa,
                IdCanalAtencion = idNroMesa.HasValue ? Canales.Salon : canal,
                IdCliente = idCliente
            };
            vm.Detalles.Add(new DetallePedidoVM());
            await CargarListasAsync(vm);
            return View(vm);
        }

        // POST: /Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoCreateVM vm)
        {
            if (!ModelState.IsValid) return await VolverAlFormularioAsync(vm);

            var r = await _pedidos.RegistrarAsync(ASolicitud(vm), UsuarioActual);
            if (!r.Exito)
            {
                CopiarErrores(r, ModelState);
                return await VolverAlFormularioAsync(vm);   // conserva detalles, dirección, cantidades
            }

            Notificar(r);
            return RedirectToAction(nameof(Details), new { id = r.Valor });
        }

        // GET: /Pedidos/Edit/5  (solo mientras está Recibido)
        public async Task<IActionResult> Edit(int id)
        {
            var pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            if (!MaquinaEstadosPedido.PuedeEditar(pedido.IdEstadoPedido))
            {
                TempData["ErrorMessage"] = "Solo se puede editar un pedido en estado Recibido.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = new PedidoCreateVM
            {
                IdPedido = pedido.IdPedido,
                IdCliente = pedido.IdCliente,
                IdCanalAtencion = pedido.IdCanalAtencion,
                IdNroMesa = pedido.PedidoLocal?.IdNroMesa,
                IdZonaDelivery = pedido.PedidoDelivery?.IdZonaDelivery,
                Direccion = pedido.PedidoDelivery?.Direccion,
                Referencia = pedido.PedidoDelivery?.Referencia,
                Telefono = pedido.PedidoDelivery?.Telefono,
                Detalles = pedido.DetallePedidos
                    .Select(d => new DetallePedidoVM { IdProducto = d.IdProducto, IdSabor = d.IdSabor, Cantidad = d.Cantidad })
                    .ToList()
            };
            await CargarListasAsync(vm);
            return View("Create", vm);
        }

        // POST: /Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PedidoCreateVM vm)
        {
            vm.IdPedido = id;
            if (!ModelState.IsValid) return await VolverAlFormularioAsync(vm);

            var r = await _pedidos.EditarAsync(id, ASolicitud(vm), UsuarioActual);
            if (!r.Exito)
            {
                CopiarErrores(r, ModelState);
                return await VolverAlFormularioAsync(vm);
            }

            Notificar(r);
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Pedidos/MarcarServido/5  (Listo → Servido en salón)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarServido(int id, string? volver)
        {
            Notificar(await _pedidos.CambiarEstadoAsync(id, Estados.Servido, UsuarioActual));
            return volver == "salon" ? RedirectToAction("Index", "Salon") : RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Pedidos/Cobrar/5
        public async Task<IActionResult> Cobrar(int id)
        {
            var pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            var vm = new CobroVM { IdPedido = id, Pedido = pedido };
            await CargarTiposPagoAsync(vm);
            return View(vm);
        }

        // POST: /Pedidos/Cobrar/5  (Servido → Entregado, con boleta o factura)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cobrar(int id, CobroVM vm)
        {
            vm.IdPedido = id;
            if (ModelState.IsValid)
            {
                var r = await _pedidos.CobrarAsync(id, ASolicitudCobro(vm), UsuarioActual);
                if (r.Exito)
                {
                    Notificar(r);
                    return RedirectToAction(nameof(Comprobante), new { id });
                }
                CopiarErrores(r, ModelState);
            }
            vm.Pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (vm.Pedido == null) return NotFound();
            await CargarTiposPagoAsync(vm);
            return View(vm);
        }

        // GET: /Pedidos/Comprobante/5
        public async Task<IActionResult> Comprobante(int id)
        {
            var pedido = await _pedidos.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            if (pedido.Pago == null)
            {
                TempData["ErrorMessage"] = "El pedido aún no tiene comprobante: primero registra el cobro.";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(new ComprobanteVM { Pedido = pedido, Desglose = _igv.Desglosar(pedido.Total), Negocio = _config });
        }

        // POST: /Pedidos/Anular/5 — solo Administrador con segundo factor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Politicas.AnularPedido)]
        public async Task<IActionResult> Anular(int id, string? motivo)
        {
            Notificar(await _pedidos.AnularAsync(id, motivo, UsuarioActual));
            return RedirectToAction(nameof(Details), new { id });
        }

        // ---------------- apoyo ----------------
        private static SolicitudPedido ASolicitud(PedidoCreateVM vm) => new()
        {
            IdCliente = vm.IdCliente ?? 0,
            IdCanalAtencion = vm.IdCanalAtencion ?? 0,
            IdNroMesa = vm.IdNroMesa,
            IdZonaDelivery = vm.IdZonaDelivery,
            Direccion = vm.Direccion,
            Referencia = vm.Referencia,
            Telefono = vm.Telefono,
            Lineas = vm.Detalles
                .Select(d => new LineaPedido { IdProducto = d.IdProducto, IdSabor = d.IdSabor, Cantidad = d.Cantidad })
                .ToList()
        };

        public static SolicitudCobro ASolicitudCobro(CobroVM vm) => new()
        {
            IdTipoPago = vm.IdTipoPago ?? 0,
            TipoComprobante = vm.TipoComprobante,
            Ruc = vm.Ruc,
            RazonSocial = vm.RazonSocial
        };

        private async Task<IActionResult> VolverAlFormularioAsync(PedidoCreateVM vm)
        {
            if (vm.Detalles.Count == 0) vm.Detalles.Add(new DetallePedidoVM());
            await CargarListasAsync(vm);
            return View("Create", vm);
        }

        private async Task CargarTiposPagoAsync(CobroVM vm)
        {
            vm.TiposPago = (await _catalogo.ListarTiposPagoAsync())
                .Select(t => new SelectListItem(t.Nombre, t.IdTipoPago.ToString()));
        }

        private async Task CargarListasAsync(PedidoCreateVM vm)
        {
            vm.Clientes = (await _catalogo.ListarClientesAsync())
                .Select(c => new SelectListItem($"{c.Nombre} {c.ApellidoPaterno} · DNI {c.DNI}", c.IdCliente.ToString()));

            vm.Canales = (await _catalogo.ListarCanalesAsync())
                .Select(c => new SelectListItem(c.Nombre, c.IdCanalAtencion.ToString()));

            vm.Zonas = (await _catalogo.ListarZonasDeliveryAsync())
                .Select(z => new SelectListItem($"{z.NombreZona} (S/ {z.CostoDelivery:0.00})", z.IdZonaDelivery.ToString()));

            vm.Mesas = (await _catalogo.ListarMesasAsync())
                .Select(m => new SelectListItem($"Mesa {m.NumeroMesa} · {m.ZonaLocal?.Nombre}", m.IdNroMesa.ToString()));

            var productos = await _catalogo.ListarProductosParaPedidoAsync();
            vm.Productos = productos.Select(p => new SelectListItem($"{p.Nombre} - S/ {p.Precio:0.00}", p.IdProducto.ToString()));
            vm.ProductoRequiereSabor = productos.ToDictionary(p => p.IdProducto, p => p.RequiereSabor);

            vm.Sabores = (await _catalogo.ListarSaboresAsync())
                .Select(s => new SelectListItem(s.Nombre, s.IdSabor.ToString()));
        }
    }
}
