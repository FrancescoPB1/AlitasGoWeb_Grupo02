using System.Text.RegularExpressions;
using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // GRASP Controlador: coordina los casos de uso del pedido. El PedidosController solo traduce HTTP.
    public class PedidoService : IPedidoService
    {
        public const int CantidadMaximaPorLinea = 50;

        private readonly IPedidoRepository _repo;
        private readonly ICatalogoRepository _catalogo;
        private readonly IPrecioService _precios;
        private readonly IInventarioService _inventario;
        private readonly IAuditoriaService _auditoria;
        private readonly IUnidadDeTrabajo _uow;
        private readonly IReloj _reloj;
        private readonly IConfiguracionSistema _config;
        private readonly INotificadorPedidos _notificador;

        public PedidoService(
            IPedidoRepository repo,
            ICatalogoRepository catalogo,
            IPrecioService precios,
            IInventarioService inventario,
            IAuditoriaService auditoria,
            IUnidadDeTrabajo uow,
            IReloj reloj,
            IConfiguracionSistema config,
            INotificadorPedidos notificador)
        {
            _repo = repo;
            _catalogo = catalogo;
            _precios = precios;
            _inventario = inventario;
            _auditoria = auditoria;
            _uow = uow;
            _reloj = reloj;
            _config = config;
            _notificador = notificador;
        }

        // Aviso en tiempo real DESPUÉS de confirmar la transacción.
        private Task NotificarAsync(Pedido p, string evento) =>
            _notificador.PedidoActualizadoAsync(new AvisoPedido(
                p.IdPedido, p.IdCanalAtencion, p.IdEstadoPedido, evento, p.PedidoLocal?.NroMesa?.NumeroMesa));

        // ======================= Registrar =======================
        public async Task<Resultado<int>> RegistrarAsync(SolicitudPedido s, string usuario)
        {
            var pedido = new Pedido
            {
                Fecha = _reloj.Ahora,
                IdCliente = s.IdCliente,
                IdCanalAtencion = s.IdCanalAtencion,
                IdEstadoPedido = Estados.Recibido,
                UsuarioRegistro = usuario
            };

            var errores = await ConstruirAsync(s, pedido);
            if (errores.Count > 0) return Resultado<int>.Error(errores.ToArray());

            // Validar stock (<<include>> del caso de uso Registrar pedido)
            var consumo = await _inventario.CalcularConsumoAsync(pedido.DetallePedidos);
            var faltantes = await _inventario.ValidarStockAsync(consumo);
            if (faltantes.Count > 0) return Resultado<int>.Error(faltantes.ToArray());

            // Maestro, detalles y movimientos de inventario en UNA sola transacción.
            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                await _repo.AgregarAsync(pedido);
                await _inventario.DescontarAsync(consumo, "Pedido registrado", pedido, usuario);
            });

            await NotificarAsync(pedido, EventosPedido.Registrado);
            return Resultado<int>.Ok(pedido.IdPedido, $"Pedido #{pedido.IdPedido} registrado y enviado a cocina.");
        }

        // ======================= Editar (solo en Recibido) =======================
        public async Task<Resultado> EditarAsync(int idPedido, SolicitudPedido s, string usuario)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return Resultado.Error("El pedido no existe.");
            if (!MaquinaEstadosPedido.PuedeEditar(pedido.IdEstadoPedido))
                return Resultado.Error("Solo se puede editar un pedido en estado Recibido (antes de que la cocina lo empiece).");

            // El canal no cambia: un pedido de salón no se vuelve delivery.
            s.IdCanalAtencion = pedido.IdCanalAtencion;

            // Se arma un borrador para validar antes de tocar el pedido real.
            var borrador = new Pedido
            {
                Fecha = pedido.Fecha,
                IdCliente = s.IdCliente,
                IdCanalAtencion = pedido.IdCanalAtencion,
                IdEstadoPedido = pedido.IdEstadoPedido
            };
            var errores = await ConstruirAsync(s, borrador);
            if (errores.Count > 0) return Resultado.Error(errores.ToArray());

            var consumoAnterior = await _inventario.CalcularConsumoAsync(pedido.DetallePedidos);
            var consumoNuevo = await _inventario.CalcularConsumoAsync(borrador.DetallePedidos);
            var neto = InventarioService.Diferencia(consumoNuevo, consumoAnterior);
            var faltantes = await _inventario.ValidarStockAsync(neto);
            if (faltantes.Count > 0) return Resultado.Error(faltantes.ToArray());

            var lineasAntes = pedido.DetallePedidos.Count;
            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                _repo.EliminarDetalles(pedido.DetallePedidos.ToList());
                pedido.DetallePedidos.Clear();
                foreach (var d in borrador.DetallePedidos) pedido.DetallePedidos.Add(d);

                pedido.IdCliente = s.IdCliente;
                if (pedido.EsDelivery && pedido.PedidoDelivery != null && borrador.PedidoDelivery != null)
                {
                    pedido.PedidoDelivery.IdZonaDelivery = borrador.PedidoDelivery.IdZonaDelivery;
                    pedido.PedidoDelivery.Direccion = borrador.PedidoDelivery.Direccion;
                    pedido.PedidoDelivery.Referencia = borrador.PedidoDelivery.Referencia;
                    pedido.PedidoDelivery.Telefono = borrador.PedidoDelivery.Telefono;
                    pedido.PedidoDelivery.CostoEnvio = borrador.PedidoDelivery.CostoEnvio;
                }
                else if (pedido.PedidoLocal != null && borrador.PedidoLocal != null)
                {
                    pedido.PedidoLocal.IdNroMesa = borrador.PedidoLocal.IdNroMesa;
                }
                pedido.Total = borrador.Total;

                await _inventario.DescontarAsync(neto.Where(c => c.Cantidad > 0), "Edición de pedido", pedido, usuario);
                await _inventario.ReponerAsync(neto.Where(c => c.Cantidad < 0).Select(c => new ConsumoInsumo(c.IdInsumo, -c.Cantidad)),
                    "Edición de pedido", pedido, usuario);
                await _auditoria.RegistrarAsync(usuario, "Editar pedido", "Pedido", idPedido.ToString(),
                    $"Líneas: {lineasAntes} → {borrador.DetallePedidos.Count}. Total S/ {pedido.Total:0.00}");
            });

            await NotificarAsync(pedido, EventosPedido.Editado);
            return Resultado.Ok($"Pedido #{idPedido} actualizado.");
        }

        // Valida la solicitud y arma el pedido (detalles, precios, datos de canal y total).
        private async Task<List<string>> ConstruirAsync(SolicitudPedido s, Pedido pedido)
        {
            var errores = new List<string>();

            var cliente = await _catalogo.ObtenerClienteAsync(s.IdCliente);
            if (cliente == null || !cliente.Activo) errores.Add("Selecciona un cliente válido.");

            decimal costoEnvio = 0;
            if (s.IdCanalAtencion == Canales.Salon)
            {
                var mesa = s.IdNroMesa.HasValue ? await _catalogo.ObtenerMesaAsync(s.IdNroMesa.Value) : null;
                if (mesa == null || !mesa.Activo) errores.Add("Selecciona una mesa.");
                else pedido.PedidoLocal = new PedidoLocal { IdNroMesa = mesa.IdNroMesa };
            }
            else if (s.IdCanalAtencion == Canales.Delivery)
            {
                var zona = s.IdZonaDelivery.HasValue ? await _catalogo.ObtenerZonaDeliveryAsync(s.IdZonaDelivery.Value) : null;
                if (zona == null || !zona.Activo) errores.Add("Selecciona la zona de delivery.");
                if (string.IsNullOrWhiteSpace(s.Direccion)) errores.Add("Ingresa la dirección de entrega.");
                if (string.IsNullOrWhiteSpace(s.Telefono)) errores.Add("Ingresa un teléfono de contacto para el repartidor.");
                if (zona != null && zona.Activo)
                {
                    costoEnvio = zona.CostoDelivery;
                    pedido.PedidoDelivery = new PedidoDelivery
                    {
                        IdZonaDelivery = zona.IdZonaDelivery,
                        Direccion = s.Direccion?.Trim(),
                        Referencia = string.IsNullOrWhiteSpace(s.Referencia) ? null : s.Referencia.Trim(),
                        Telefono = s.Telefono?.Trim(),
                        CostoEnvio = zona.CostoDelivery
                    };
                }
            }
            else
            {
                errores.Add("Selecciona un canal de atención.");
            }

            // Filas vacías del formulario (sin producto) se ignoran; el resto se valida.
            var lineas = s.Lineas.Where(l => l.IdProducto > 0).ToList();
            if (lineas.Count == 0)
            {
                errores.Add("Agrega al menos un producto.");
                return errores;
            }

            var productos = await _catalogo.ObtenerProductosAsync(lineas.Select(l => l.IdProducto));
            var sabores = await _catalogo.ListarSaboresAsync(true);

            foreach (var l in lineas)
            {
                var producto = productos.FirstOrDefault(p => p.IdProducto == l.IdProducto);
                if (producto == null || !producto.Activo)
                {
                    errores.Add("Uno de los productos ya no está disponible.");
                    continue;
                }
                if (l.Cantidad < 1 || l.Cantidad > CantidadMaximaPorLinea)
                {
                    errores.Add($"La cantidad de «{producto.Nombre}» debe estar entre 1 y {CantidadMaximaPorLinea}.");
                    continue;
                }
                if (producto.RequiereSabor)
                {
                    if (!l.IdSabor.HasValue)
                    {
                        errores.Add($"Elige el sabor de «{producto.Nombre}».");
                        continue;
                    }
                    if (sabores.All(x => x.IdSabor != l.IdSabor.Value))
                    {
                        errores.Add($"El sabor elegido para «{producto.Nombre}» no está disponible.");
                        continue;
                    }
                }
                if (errores.Count > 0) continue;   // no se calculan precios si ya hay errores

                var detalle = pedido.AgregarDetalle(producto, l.Cantidad, l.IdSabor);
                await _precios.AplicarPrecioAsync(detalle, producto, pedido.IdCanalAtencion, pedido.Fecha);
            }

            if (errores.Count == 0) pedido.CalcularTotal(costoEnvio);
            return errores;
        }

        // ======================= Consultas =======================
        public Task<Pedido?> ObtenerConDetallesAsync(int idPedido) => _repo.ObtenerConDetallesAsync(idPedido);

        public Task<List<Pedido>> ListarAsync(DateTime? fecha, int? idEstado)
        {
            var dia = (fecha ?? _reloj.Ahora).Date;
            return _repo.ListarAsync(dia, dia.AddDays(1), idEstado);
        }

        public Task<List<Pedido>> ListarCocinaAsync() =>
            _repo.ListarPorEstadosAsync(new[] { Estados.Recibido, Estados.EnPreparacion }, null);

        public Task<List<Pedido>> ListarRepartoAsync() =>
            _repo.ListarPorEstadosAsync(new[] { Estados.Listo, Estados.EnReparto }, Canales.Delivery);

        public Task<List<Pedido>> ListarActivosSalonAsync() =>
            _repo.ListarPorEstadosAsync(new[] { Estados.Recibido, Estados.EnPreparacion, Estados.Listo, Estados.Servido }, Canales.Salon);

        // ======================= Estados =======================
        public async Task<Resultado> CambiarEstadoAsync(int idPedido, int nuevoEstado, string usuario)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return Resultado.Error("El pedido no existe.");

            if (nuevoEstado == Estados.Entregado)
                return Resultado.Error("Para cerrar el pedido registra el cobro.");
            if (nuevoEstado == Estados.Anulado)
                return Resultado.Error("Para anular usa la opción Anular pedido.");

            if (!MaquinaEstadosPedido.TransicionValida(pedido.IdEstadoPedido, nuevoEstado, pedido.EsDelivery))
                return Resultado.Error(
                    $"No se puede pasar el pedido #{idPedido} de «{MaquinaEstadosPedido.Nombre(pedido.IdEstadoPedido)}» " +
                    $"a «{MaquinaEstadosPedido.Nombre(nuevoEstado)}».");

            pedido.IdEstadoPedido = nuevoEstado;
            await _uow.GuardarCambiosAsync();
            await NotificarAsync(pedido, EventosPedido.Estado);
            return Resultado.Ok($"Pedido #{idPedido}: {MaquinaEstadosPedido.Nombre(nuevoEstado)}.");
        }

        public async Task<Resultado> AnularAsync(int idPedido, string? motivo, string usuario)
        {
            motivo = motivo?.Trim();
            if (string.IsNullOrEmpty(motivo) || motivo.Length < 5)
                return Resultado.Error("Indica el motivo de la anulación (mínimo 5 caracteres).");
            if (motivo.Length > 200) motivo = motivo[..200];

            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return Resultado.Error("El pedido no existe.");

            var estadoPrevio = pedido.IdEstadoPedido;
            if (!MaquinaEstadosPedido.PuedeAnular(estadoPrevio))
                return Resultado.Error($"No se puede anular un pedido {MaquinaEstadosPedido.Nombre(estadoPrevio).ToLower()}.");

            var reponer = MaquinaEstadosPedido.DebeReponerStock(estadoPrevio);
            var consumo = reponer ? await _inventario.CalcularConsumoAsync(pedido.DetallePedidos) : new List<ConsumoInsumo>();

            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                pedido.IdEstadoPedido = Estados.Anulado;
                pedido.MotivoAnulacion = motivo;
                if (reponer) await _inventario.ReponerAsync(consumo, "Anulación de pedido", pedido, usuario);
                await _auditoria.RegistrarAsync(usuario, "Anular pedido", "Pedido", idPedido.ToString(),
                    $"Estado previo: {MaquinaEstadosPedido.Nombre(estadoPrevio)}. Total S/ {pedido.Total:0.00}. " +
                    $"Stock repuesto: {(reponer ? "sí" : "no")}. Motivo: {motivo}");
            });

            await NotificarAsync(pedido, EventosPedido.Anulado);
            return Resultado.Ok($"Pedido #{idPedido} anulado.");
        }

        // ======================= Cobro y comprobante =======================
        public async Task<Resultado> CobrarAsync(int idPedido, SolicitudCobro cobro, string usuario)
        {
            var pedido = await _repo.ObtenerConDetallesAsync(idPedido);
            if (pedido == null) return Resultado.Error("El pedido no existe.");
            if (pedido.Pago != null) return Resultado.Error("El pedido ya fue cobrado.");

            if (!MaquinaEstadosPedido.PuedeCobrar(pedido.IdEstadoPedido, pedido.EsDelivery))
                return Resultado.Error(pedido.EsDelivery
                    ? "Solo se cobra un delivery cuando está en reparto (pago contra entrega)."
                    : "Primero marca el pedido como servido en la mesa.");

            var errores = new List<string>();
            var tipoPago = await _catalogo.ObtenerTipoPagoAsync(cobro.IdTipoPago);
            if (tipoPago == null || !tipoPago.Activo) errores.Add("Selecciona el medio de pago.");

            var esFactura = cobro.TipoComprobante == TiposComprobante.Factura;
            if (!esFactura && cobro.TipoComprobante != TiposComprobante.Boleta)
                errores.Add("Selecciona boleta o factura.");
            if (esFactura)
            {
                if (string.IsNullOrWhiteSpace(cobro.Ruc) || !Regex.IsMatch(cobro.Ruc.Trim(), @"^\d{11}$"))
                    errores.Add("Para factura el RUC debe tener 11 dígitos.");
                if (string.IsNullOrWhiteSpace(cobro.RazonSocial))
                    errores.Add("Para factura indica la razón social.");
            }
            if (errores.Count > 0) return Resultado.Error(errores.ToArray());

            var serie = esFactura ? _config.SerieFactura : _config.SerieBoleta;
            await _uow.EjecutarEnTransaccionAsync(() =>
            {
                pedido.Pago = new Pago
                {
                    IdPedido = pedido.IdPedido,
                    IdTipoPago = cobro.IdTipoPago,
                    FechaPago = _reloj.Ahora,
                    Monto = pedido.Total,
                    TipoComprobante = esFactura ? TiposComprobante.Factura : TiposComprobante.Boleta,
                    NumeroComprobante = $"{serie}-{pedido.IdPedido:D8}",
                    DocumentoCliente = esFactura ? cobro.Ruc!.Trim() : pedido.Cliente?.DNI,
                    RazonSocial = esFactura ? cobro.RazonSocial!.Trim() : null,
                    UsuarioCobro = usuario
                };
                pedido.IdEstadoPedido = Estados.Entregado;
                return Task.CompletedTask;
            });

            await NotificarAsync(pedido, EventosPedido.Cobrado);
            return Resultado.Ok($"Pedido #{idPedido} cobrado ({tipoPago!.Nombre}). Comprobante {serie}-{idPedido:D8}.");
        }
    }
}
