using AlitasGoWeb.Data;
using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Precios;

namespace AlitasGoWeb.Services
{
    public class PedidoService:IPedidoService
    {
        private readonly IPedidoRepository _repo;
        private readonly IEnumerable<IEstrategiaPrecio> _estrategias;
        private readonly IPromocionService _promocionService;
        private readonly AlitasGoDbContext _ctx;

        public PedidoService(
            IPedidoRepository repo,
            IEnumerable<IEstrategiaPrecio> estrategias,
            IPromocionService promocionService,
            AlitasGoDbContext ctx)
        {
            _repo = repo;
            _estrategias = estrategias;
            _promocionService = promocionService;
            _ctx = ctx;
        }

        public async Task<int> RegistrarPedidoAsync(Pedido pedido, List<DetallePedido> detalles)
        {
            decimal total = 0;

            foreach (var d in detalles)
            {
                var codigo = await _promocionService.ObtenerCodigoAplicableAsync(
                    d.IdProducto, pedido.IdCanalAtencion, pedido.Fecha)
                    ?? "NORMAL";

                var estrategia = _estrategias.First(e => e.Clave == codigo);
                var precioFinal = estrategia.CalcularPrecio(d);

                d.SubTotal = precioFinal;
                d.Descuento = (d.PrecioUnitario * d.Cantidad) - precioFinal;
                total += d.SubTotal;
            }

            // Sumar costo de delivery al total
            if (pedido.IdCanalAtencion == 2 && pedido.PedidoDelivery != null)
            {
                var zona = await _ctx.ZonasDelivery
                    .FindAsync(pedido.PedidoDelivery.IdZonaDelivery);

                if (zona != null)
                    total += zona.CostoDelivery;
            }

            pedido.Total = total;
            pedido.DetallePedidos = detalles;

            await _repo.AgregarAsync(pedido);
            await _repo.GuardarAsync();

            return pedido.IdPedido;
        }

        public Task<Pedido?> ObtenerConDetallesAsync(int idPedido)
            => _repo.ObtenerConDetallesAsync(idPedido);

        public Task<List<Pedido>> ListarAsync()
            => _repo.ObtenerTodosAsync();

        public async Task CambiarEstadoAsync(int idPedido, int nuevoEstadoId)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return;

            // 1) Validar transición básica
            if (!TransicionValida(pedido.IdEstadoPedido, nuevoEstadoId))
                throw new InvalidOperationException(
                    $"Transición no permitida: {pedido.IdEstadoPedido} → {nuevoEstadoId}");

            // 2) Si el pedido es delivery, cuando está Listo SOLO puede ir a EnReparto
            bool esDelivery = pedido.PedidoDelivery != null;

            if (pedido.IdEstadoPedido == 3 && esDelivery && nuevoEstadoId != 4)
                throw new InvalidOperationException(
                    "Un pedido de delivery en estado Listo debe pasar a EnReparto.");

            // 3) Si el pedido es local, NO puede ir a EnReparto
            if (!esDelivery && nuevoEstadoId == 4)
                throw new InvalidOperationException(
                    "Un pedido de salón no puede pasar a EnReparto.");

            pedido.IdEstadoPedido = nuevoEstadoId;
            await _repo.GuardarAsync();
        }

        public async Task AnularAsync(int idPedido)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return;

            bool esDelivery = pedido.PedidoDelivery != null;

            if (esDelivery)
            {
                // Delivery: se puede anular desde cualquier estado excepto Entregado (5) y Anulado (6)
                if (pedido.IdEstadoPedido == 5 || pedido.IdEstadoPedido == 6)
                    throw new InvalidOperationException(
                        "No se puede anular un pedido de delivery ya entregado o anulado.");
            }
            else
            {
                // Salón: solo desde Recibido (1) o EnPreparacion (2)
                if (pedido.IdEstadoPedido >= 3)
                    throw new InvalidOperationException(
                        "No se puede anular un pedido de salón que ya está listo o entregado.");
            }

            pedido.IdEstadoPedido = 6;
            await _repo.GuardarAsync();
        }

        // Reglas de transición del PDF:
        // Recibido → EnPreparacion → Listo → (EnReparto | Servido) → Entregado
        // Desde cualquier estado antes de Entregado se puede ir a Anulado
        private bool TransicionValida(int actual, int nuevo)
        {
            return actual switch
            {
                1 => nuevo is 2 or 6,       // Recibido → EnPreparacion | Anulado
                2 => nuevo is 3 or 6,       // EnPreparacion → Listo | Anulado
                3 => nuevo is 4 or 5,       // Listo → EnReparto | Entregado (NO anular)
                4 => nuevo is 5,            // EnReparto → Entregado (NO anular)
                5 => false,                 // Entregado: final
                6 => false,                 // Anulado: final
                _ => false
            };
        }
    }
}
