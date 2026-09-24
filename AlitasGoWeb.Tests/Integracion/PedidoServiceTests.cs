using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using static AlitasGoWeb.Tests.Infraestructura.Escenario;

namespace AlitasGoWeb.Tests.Integracion
{
    // Insumos sembrados: 1 Alitas crudas 20 kg · 2 Salsa buffalo 5 lt · 3 Salsa BBQ 4 lt
    //                    4 Gaseosa personal 50 u · 5 Salsa acevichada 3 lt · 6 Papas 15 kg
    public class PedidoServiceTests
    {
        // ------------------- Registrar -------------------
        [Fact]
        public async Task Registrar_PedidoDeSalon_CalculaTotalDescuentaStockYDejaMovimientos()
        {
            using var e = new Escenario(Dias.Lunes);

            var id = await e.RegistrarAsync(Salon(3,
                Linea(2, 2, idSabor: 2),   // 2 docenas BBQ = 60
                Linea(6, 1),               // papas = 8
                Linea(9, 1)));             // gaseosa = 5

            var pedido = e.Leer(id);
            Assert.Equal(73m, pedido.Total);
            Assert.Equal(Estados.Recibido, pedido.IdEstadoPedido);
            Assert.Equal(3, pedido.PedidoLocal!.IdNroMesa);
            Assert.Equal(Usuario, pedido.UsuarioRegistro);
            Assert.Equal(3, pedido.DetallePedidos.Count);

            Assert.Equal(17.6m, e.Stock(1));   // 20 - 2 × 1.2
            Assert.Equal(3.8m, e.Stock(3));    // 4 - 2 × 0.1 (solo BBQ)
            Assert.Equal(5m, e.Stock(2));      // buffalo intacta
            Assert.Equal(14.75m, e.Stock(6));
            Assert.Equal(49m, e.Stock(4));

            using var ctx = e.NuevoContexto();
            var movimientos = ctx.MovimientosInventario.Where(m => m.IdPedido == id).ToList();
            Assert.Equal(4, movimientos.Count);
            Assert.All(movimientos, m => Assert.Equal(TiposMovimiento.Salida, m.Tipo));
        }

        [Fact]
        public async Task Registrar_Delivery_SumaElCostoDeEnvioDeLaZona()
        {
            using var e = new Escenario(Dias.Lunes);

            var id = await e.RegistrarAsync(Delivery(2, Linea(1, 1, idSabor: 1)));   // 18 + zona Media 5

            var pedido = e.Leer(id);
            Assert.Equal(23m, pedido.Total);
            Assert.Equal(5m, pedido.PedidoDelivery!.CostoEnvio);
            Assert.Equal("987654321", pedido.PedidoDelivery.Telefono);
            Assert.Equal("Portón azul, frente a la bodega", pedido.PedidoDelivery.Referencia);
        }

        [Fact]
        public async Task Registrar_MartesEnSalon_Aplica2x1EnMediaDocena()
        {
            using var e = new Escenario(Dias.Martes);

            var id = await e.RegistrarAsync(Salon(1, Linea(1, 3, idSabor: 1)));

            var detalle = Assert.Single(e.Leer(id).DetallePedidos);
            Assert.Equal(36m, detalle.SubTotal);   // paga 2 de 3
            Assert.Equal(18m, detalle.Descuento);
        }

        [Fact]
        public async Task Registrar_MartesPorDelivery_NoAplica2x1PorqueEsSoloSalon()
        {
            using var e = new Escenario(Dias.Martes);

            var id = await e.RegistrarAsync(Delivery(1, Linea(1, 2, idSabor: 1)));

            var pedido = e.Leer(id);
            Assert.Equal(0m, pedido.DetallePedidos.Single().Descuento);
            Assert.Equal(39m, pedido.Total);   // 36 + 3
        }

        [Fact]
        public async Task Registrar_Jueves_AplicaDocenaDelDia()
        {
            using var e = new Escenario(Dias.Jueves);

            var id = await e.RegistrarAsync(Salon(2, Linea(2, 1, idSabor: 3)));

            var detalle = e.Leer(id).DetallePedidos.Single();
            Assert.Equal(25.50m, detalle.SubTotal);
            Assert.Equal(4.50m, detalle.Descuento);
        }

        [Fact]
        public async Task Registrar_Miercoles_AplicaPromocionCreadaParaEseDia()
        {
            // Regresión del bug de la tilde: la promoción de miércoles antes nunca aplicaba.
            using var e = new Escenario(Dias.Miercoles);
            var creada = await e.Promociones.CrearAsync(new SolicitudPromocion
            {
                Nombre = "Miércoles 2x1 en papas",
                IdTipoPromocion = 3,
                IdCanalPromocion = 3,
                IdDia = 3,
                IdsProducto = new List<int> { 6 }
            }, Admin);
            Assert.True(creada.Exito);

            var id = await e.RegistrarAsync(Salon(4, Linea(6, 2)));

            Assert.Equal(8m, e.Leer(id).Total);
        }

        [Fact]
        public async Task Registrar_Combo_CobraPrecioDeComboYDescuentaInsumosDeSusComponentes()
        {
            using var e = new Escenario(Dias.Jueves);   // aunque el jueves hay promo en docena, el combo no acumula

            var id = await e.RegistrarAsync(Salon(5, Linea(13, 1)));

            var detalle = e.Leer(id).DetallePedidos.Single();
            Assert.Equal(65m, detalle.SubTotal);
            Assert.Equal(0m, detalle.Descuento);
            Assert.Equal(17.6m, e.Stock(1));   // 2 docenas
            Assert.Equal(4.8m, e.Stock(2));    // sabor Buffalo definido en el combo
            Assert.Equal(48m, e.Stock(4));     // 2 gaseosas
        }

        [Fact]
        public async Task Registrar_FilasVaciasDelFormulario_SeIgnoran()
        {
            // Regresión: una fila sin producto provocaba KeyNotFoundException.
            using var e = new Escenario();

            var id = await e.RegistrarAsync(Salon(1, Linea(0, 1), Linea(9, 2)));

            Assert.Equal(10m, e.Leer(id).Total);
        }

        [Fact]
        public async Task Registrar_ProductoConSaborSinElegirlo_DevuelveError()
        {
            using var e = new Escenario();

            var r = await e.Pedidos.RegistrarAsync(Salon(1, Linea(2, 1)), Usuario);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("sabor"));
            Assert.Empty(e.NuevoContexto().Pedidos);
        }

        [Fact]
        public async Task Registrar_DeliverySinDireccionNiTelefono_DevuelveErrores()
        {
            using var e = new Escenario();
            var s = Delivery(1, Linea(9, 1));
            s.Direccion = " ";
            s.Telefono = null;

            var r = await e.Pedidos.RegistrarAsync(s, Usuario);

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("dirección"));
            Assert.Contains(r.Errores, x => x.Contains("teléfono"));
        }

        [Fact]
        public async Task Registrar_SinStockSuficiente_NoGuardaNada()
        {
            using var e = new Escenario();

            var r = await e.Pedidos.RegistrarAsync(Salon(1, Linea(3, 10, idSabor: 4)), Usuario);   // 24 kg > 20 kg

            Assert.False(r.Exito);
            Assert.Contains(r.Errores, x => x.Contains("Alitas crudas"));
            Assert.Empty(e.NuevoContexto().Pedidos);
            Assert.Empty(e.NuevoContexto().MovimientosInventario);
            Assert.Equal(20m, e.Stock(1));
        }

        // ------------------- Editar -------------------
        [Fact]
        public async Task Editar_QuitandoUnaLinea_ReponeElInsumoYRecalculaElTotal()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(2, Linea(2, 1, idSabor: 2), Linea(6, 1)));
            Assert.Equal(14.75m, e.Stock(6));

            var r = await e.Pedidos.EditarAsync(id, Salon(2, Linea(2, 1, idSabor: 2)), Usuario);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            var pedido = e.Leer(id);
            Assert.Equal(30m, pedido.Total);
            Assert.Single(pedido.DetallePedidos);
            Assert.Equal(15m, e.Stock(6));
            Assert.Equal(18.8m, e.Stock(1));   // la docena sigue descontada
        }

        [Fact]
        public async Task Editar_CuandoLaCocinaYaEmpezo_NoSePermite()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(2, Linea(9, 1)));
            await e.LlevarAAsync(id, Estados.EnPreparacion);

            var r = await e.Pedidos.EditarAsync(id, Salon(2, Linea(9, 3)), Usuario);

            Assert.False(r.Exito);
            Assert.Equal(5m, e.Leer(id).Total);
        }

        // ------------------- Estados, anulación y cobro -------------------
        [Fact]
        public async Task FlujoSalon_DeRecibidoAEntregadoConBoleta()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(6, Linea(1, 1, idSabor: 5)));

            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo);
            Assert.False((await e.Pedidos.CambiarEstadoAsync(id, Estados.EnReparto, Usuario)).Exito);   // salón no va a reparto
            Assert.False((await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 1 }, Usuario)).Exito); // aún no servido
            await e.LlevarAAsync(id, Estados.Servido);
            Assert.False((await e.Pedidos.CambiarEstadoAsync(id, Estados.Entregado, Usuario)).Exito); // requiere cobro

            var cobro = await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 2, TipoComprobante = TiposComprobante.Boleta }, Usuario);

            Assert.True(cobro.Exito, string.Join(" | ", cobro.Errores));
            var pedido = e.Leer(id);
            Assert.Equal(Estados.Entregado, pedido.IdEstadoPedido);
            Assert.Equal(18m, pedido.Pago!.Monto);
            Assert.Equal($"B001-{id:D8}", pedido.Pago.NumeroComprobante);
            Assert.Equal("12345678", pedido.Pago.DocumentoCliente);
        }

        [Fact]
        public async Task FlujoDelivery_CobroContraEntregaConFactura()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Delivery(3, Linea(5, 1, idSabor: 6)));   // 35 + 8
            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo);
            Assert.False((await e.Pedidos.CambiarEstadoAsync(id, Estados.Servido, Usuario)).Exito);
            await e.LlevarAAsync(id, Estados.EnReparto);

            var sinRuc = await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 1, TipoComprobante = TiposComprobante.Factura, Ruc = "123" }, Usuario);
            Assert.False(sinRuc.Exito);
            Assert.Contains(sinRuc.Errores, x => x.Contains("RUC"));

            var ok = await e.Pedidos.CobrarAsync(id, new SolicitudCobro
            {
                IdTipoPago = 1, TipoComprobante = TiposComprobante.Factura, Ruc = "20481234567", RazonSocial = "Eventos Trujillo SAC"
            }, Usuario);

            Assert.True(ok.Exito, string.Join(" | ", ok.Errores));
            var pedido = e.Leer(id);
            Assert.Equal(Estados.Entregado, pedido.IdEstadoPedido);
            Assert.Equal($"F001-{id:D8}", pedido.Pago!.NumeroComprobante);
            Assert.Equal(43m, pedido.Pago.Monto);
            Assert.False((await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 1 }, Usuario)).Exito); // no se cobra dos veces
        }

        [Fact]
        public async Task Anular_EnRecibido_ReponeStockYQuedaEnLaBitacora()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(7, Linea(2, 1, idSabor: 1)));
            Assert.Equal(18.8m, e.Stock(1));

            var r = await e.Pedidos.AnularAsync(id, "El cliente se retiró", Admin);

            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            var pedido = e.Leer(id);
            Assert.Equal(Estados.Anulado, pedido.IdEstadoPedido);
            Assert.Equal("El cliente se retiró", pedido.MotivoAnulacion);
            Assert.Equal(20m, e.Stock(1));
            Assert.Equal(5m, e.Stock(2));

            using var ctx = e.NuevoContexto();
            var registro = Assert.Single(ctx.Auditorias.Where(a => a.Accion == "Anular pedido"));
            Assert.Equal(Admin, registro.Usuario);
            Assert.Equal(id.ToString(), registro.IdEntidad);
            Assert.Contains("Stock repuesto: sí", registro.Detalle);
        }

        [Fact]
        public async Task Anular_PedidoYaListo_NoReponeStock()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(8, Linea(2, 1, idSabor: 1)));
            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo);

            var r = await e.Pedidos.AnularAsync(id, "Se quemó la salsa", Admin);

            Assert.True(r.Exito);
            Assert.Equal(18.8m, e.Stock(1));
        }

        [Fact]
        public async Task Anular_SinMotivoOYaEntregado_NoSePermite()
        {
            using var e = new Escenario();
            var id = await e.RegistrarAsync(Salon(9, Linea(9, 1)));

            Assert.False((await e.Pedidos.AnularAsync(id, "  ", Admin)).Exito);

            await e.LlevarAAsync(id, Estados.EnPreparacion, Estados.Listo, Estados.Servido);
            Assert.True((await e.Pedidos.CobrarAsync(id, new SolicitudCobro { IdTipoPago = 1 }, Usuario)).Exito);

            Assert.False((await e.Pedidos.AnularAsync(id, "Intento tardío", Admin)).Exito);
            Assert.Equal(Estados.Entregado, e.Leer(id).IdEstadoPedido);
        }

        // ------------------- Pantallas de trabajo -------------------
        [Fact]
        public async Task ListarCocina_MuestraRecibidosYEnPreparacionDelMasAntiguoAlMasNuevo()
        {
            using var e = new Escenario();
            var primero = await e.RegistrarAsync(Salon(1, Linea(9, 1)));
            e.Reloj.Ahora = e.Reloj.Ahora.AddMinutes(5);
            var segundo = await e.RegistrarAsync(Delivery(1, Linea(6, 1)));
            e.Reloj.Ahora = e.Reloj.Ahora.AddMinutes(5);
            var listo = await e.RegistrarAsync(Salon(2, Linea(9, 1)));
            await e.LlevarAAsync(segundo, Estados.EnPreparacion);
            await e.LlevarAAsync(listo, Estados.EnPreparacion, Estados.Listo);

            var cocina = await e.Pedidos.ListarCocinaAsync();
            var reparto = await e.Pedidos.ListarRepartoAsync();
            var salon = await e.Pedidos.ListarActivosSalonAsync();

            Assert.Equal(new[] { primero, segundo }, cocina.Select(p => p.IdPedido).ToArray());
            Assert.Empty(reparto);   // el delivery aún no está listo
            Assert.Equal(new[] { primero, listo }, salon.Select(p => p.IdPedido).ToArray());
        }
    }
}
