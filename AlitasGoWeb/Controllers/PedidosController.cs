using AlitasGoWeb.Data;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly AlitasGoDbContext _ctx;

        public PedidosController(IPedidoService pedidoService, AlitasGoDbContext ctx)
        {
            _pedidoService = pedidoService;
            _ctx = ctx;
        }

        // GET: /Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidos = await _pedidoService.ListarAsync();
            return View(pedidos);
        }

        // GET: /Pedidos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _pedidoService.ObtenerConDetallesAsync(id);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        // GET: /Pedidos/Create
        public async Task<IActionResult> Create()
        {
            var vm = new PedidoCreateVM();
            await CargarListasAsync(vm);
            vm.Detalles.Add(new DetallePedidoVM());
            return View(vm);
        }

        // POST: /Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoCreateVM vm)
        {
            // Quitar detalles vacíos de la validación
            vm.Detalles = vm.Detalles
                .Where(d => d.IdProducto > 0 && d.Cantidad > 0)
                .ToList();

            if (vm.Detalles.Count == 0)
                ModelState.AddModelError("", "Agrega al menos un producto.");

            if (vm.IdCanalAtencion == 1 && (!vm.IdNroMesa.HasValue || vm.IdNroMesa <= 0))
                ModelState.AddModelError(nameof(vm.IdNroMesa), "Selecciona una mesa.");

            if (vm.IdCanalAtencion == 2 && (string.IsNullOrWhiteSpace(vm.Direccion) || !vm.IdZonaDelivery.HasValue))
                ModelState.AddModelError(nameof(vm.Direccion), "Ingresa dirección y zona de delivery.");

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(vm);
                return View(vm);
            }

            var pedido = new Pedido
            {
                IdCliente = vm.IdCliente,
                IdCanalAtencion = vm.IdCanalAtencion,
                IdEstadoPedido = 1, // Recibido
                Fecha = DateTime.Now
            };

            if (vm.IdCanalAtencion == 2)
            {
                pedido.PedidoDelivery = new PedidoDelivery
                {
                    IdZonaDelivery = vm.IdZonaDelivery!.Value,
                    Direccion = vm.Direccion
                };
            }
            else
            {
                pedido.PedidoLocal = new PedidoLocal
                {
                    IdNroMesa = vm.IdNroMesa!.Value
                };
            }

            // Precio unitario desde la BD (no confiar en el cliente)
            var idsProductos = vm.Detalles.Select(d => d.IdProducto).Distinct().ToList();
            var precios = await _ctx.Productos
                .Where(p => idsProductos.Contains(p.IdProducto))
                .ToDictionaryAsync(p => p.IdProducto, p => p.Precio);

            var detalles = vm.Detalles.Select(d => new DetallePedido
            {
                IdProducto = d.IdProducto,
                IdSabor = d.IdSabor,
                Cantidad = d.Cantidad,
                PrecioUnitario = precios[d.IdProducto]
            }).ToList();

            await _pedidoService.RegistrarPedidoAsync(pedido, detalles);
            return RedirectToAction(nameof(Details), new { id = pedido.IdPedido });
        }

        // POST: /Pedidos/Anular/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id)
        {
            await _pedidoService.AnularAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync(PedidoCreateVM vm)
        {
            vm.Clientes = await _ctx.Clientes
                .Where(c => c.Activo)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = $"{c.Nombre} {c.ApellidoPaterno}"
                }).ToListAsync();

            vm.Canales = await _ctx.CanalesAtencion
                .Where(c => c.Activo)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCanalAtencion.ToString(),
                    Text = c.Nombre
                }).ToListAsync();

            vm.Zonas = await _ctx.ZonasDelivery
                .Where(z => z.Activo)
                .Select(z => new SelectListItem
                {
                    Value = z.IdZonaDelivery.ToString(),
                    Text = $"{z.NombreZona} (S/ {z.CostoDelivery})"
                }).ToListAsync();

            vm.Mesas = await _ctx.Mesas
                .Where(m => m.Activo)
                .Select(m => new SelectListItem
                {
                    Value = m.IdNroMesa.ToString(),
                    Text = $"Mesa {m.NumeroMesa}"
                }).ToListAsync();

            vm.Productos = await _ctx.Productos
                .Where(p => p.Activo)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProducto.ToString(),
                    Text = $"{p.Nombre} - S/ {p.Precio}"
                }).ToListAsync();

            vm.Sabores = await _ctx.Sabores
                .Where(s => s.Activo)
                .Select(s => new SelectListItem
                {
                    Value = s.IdSabor.ToString(),
                    Text = s.Nombre
                }).ToListAsync();
        }
    }
}
