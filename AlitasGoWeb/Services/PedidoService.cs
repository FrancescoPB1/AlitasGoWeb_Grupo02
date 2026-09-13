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

        public PedidoService(
            IPedidoRepository repo,
            IEnumerable<IEstrategiaPrecio> estrategias,
            IPromocionService promocionService)
        {
            _repo = repo;
            _estrategias = estrategias;
            _promocionService = promocionService;
        }

        public async Task<int> RegistrarPedidoAsync(Pedido pedido, List<DetallePedido> detalles)
        {
            decimal total = 0;

            foreach (var d in detalles)
            {
                // 1) ¿qué código de promoción aplica?
                var codigo = await _promocionService.ObtenerCodigoAplicableAsync(
                    d.IdProducto, pedido.IdCanalAtencion, pedido.Fecha)
                    ?? "NORMAL";

                // 2) elegir estrategia
                var estrategia = _estrategias.First(e => e.Clave == codigo);

                // 3) calcular
                var precioFinal = estrategia.CalcularPrecio(d);
                d.SubTotal = precioFinal;
                d.Descuento = (d.PrecioUnitario * d.Cantidad) - precioFinal;

                total += d.SubTotal;
            }

            pedido.Total = total;
            pedido.DetallePedidos = detalles;

            // 4) validación de canal
            if (pedido.IdCanalAtencion == 2) // Delivery
            {
                if (pedido.PedidoDelivery == null || string.IsNullOrEmpty(pedido.PedidoDelivery.Direccion))
                    throw new InvalidOperationException("Los pedidos de delivery requieren dirección.");
            }
            else if (pedido.IdCanalAtencion == 1) // Salón
            {
                if (pedido.PedidoLocal == null || pedido.PedidoLocal.IdNroMesa <= 0)
                    throw new InvalidOperationException("Los pedidos de salón requieren mesa.");
            }

            await _repo.AgregarAsync(pedido);
            await _repo.GuardarAsync();

            return pedido.IdPedido;
        }

        public Task<Pedido?> ObtenerConDetallesAsync(int idPedido)
            => _repo.ObtenerConDetallesAsync(idPedido);

        public Task<List<Pedido>> ListarAsync()
            => _repo.ObtenerTodosAsync();

        public async Task AnularAsync(int idPedido)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return;

            pedido.IdEstadoPedido = 6; // Anulado
            await _repo.GuardarAsync();
        }
    }
}
