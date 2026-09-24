using System.Globalization;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Precios;

namespace AlitasGoWeb.Tests.Unitarias
{
    public class EstrategiasPrecioTests
    {
        private static DetallePedido Detalle(string precio, int cantidad) =>
            new() { PrecioUnitario = decimal.Parse(precio, CultureInfo.InvariantCulture), Cantidad = cantidad };

        [Fact]
        public void PrecioNormal_MultiplicaPrecioPorCantidad()
        {
            Assert.Equal(36.00m, new PrecioNormal().CalcularPrecio(Detalle("18.00", 2)));
        }

        [Fact]
        public void PromocionDocena_Aplica15PorCientoDeDescuento()
        {
            Assert.Equal(25.50m, new PromocionDocena().CalcularPrecio(Detalle("30.00", 1)));
        }

        [Theory]
        [InlineData(1, "18.00")]
        [InlineData(2, "18.00")]
        [InlineData(3, "36.00")]
        [InlineData(4, "36.00")]
        public void Promocion2x1_CobraUnaDeCadaDos(int cantidad, string esperado)
        {
            var total = new Promocion2x1().CalcularPrecio(Detalle("18.00", cantidad));
            Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), total);
        }

        [Fact]
        public void PrecioCombo_CobraElPrecioDelCombo()
        {
            Assert.Equal(130.00m, new PrecioCombo().CalcularPrecio(Detalle("65.00", 2)));
        }

        // ------- Contexto del Strategy: PrecioService elige la estrategia -------
        private class PromocionFalsa : IPromocionService
        {
            private readonly string? _codigo;
            public PromocionFalsa(string? codigo) { _codigo = codigo; }
            public Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanal, DateTime fecha) => Task.FromResult(_codigo);
            public Task<List<Promocion>> ListarAsync() => throw new NotImplementedException();
            public Task<Resultado> CrearAsync(Services.Dtos.SolicitudPromocion s, string u) => throw new NotImplementedException();
            public Task<Resultado> AlternarActivoAsync(int id, string u) => throw new NotImplementedException();
            public Task<List<TipoPromocion>> ListarTiposAsync() => throw new NotImplementedException();
            public Task<List<CanalPromocion>> ListarCanalesAsync() => throw new NotImplementedException();
            public Task<List<Dia>> ListarDiasAsync() => throw new NotImplementedException();
        }

        private static PrecioService Servicio(string? codigoPromocion) =>
            new(new IEstrategiaPrecio[] { new PrecioNormal(), new PromocionDocena(), new Promocion2x1(), new PrecioCombo() },
                new PromocionFalsa(codigoPromocion));

        [Fact]
        public async Task PrecioService_SinPromocion_UsaPrecioNormal()
        {
            var producto = new Producto { IdProducto = 2, Precio = 30m, IdTipoProducto = 1 };
            var detalle = new DetallePedido { Cantidad = 2 };

            var clave = await Servicio(null).AplicarPrecioAsync(detalle, producto, Canales.Salon, DateTime.Today);

            Assert.Equal("NORMAL", clave);
            Assert.Equal(30m, detalle.PrecioUnitario);
            Assert.Equal(60m, detalle.SubTotal);
            Assert.Equal(0m, detalle.Descuento);
        }

        [Fact]
        public async Task PrecioService_ConPromocion2x1_CalculaSubtotalYDescuento()
        {
            var producto = new Producto { IdProducto = 1, Precio = 18m, IdTipoProducto = 1 };
            var detalle = new DetallePedido { Cantidad = 2 };

            var clave = await Servicio("2X1").AplicarPrecioAsync(detalle, producto, Canales.Salon, DateTime.Today);

            Assert.Equal("2X1", clave);
            Assert.Equal(18m, detalle.SubTotal);
            Assert.Equal(18m, detalle.Descuento);
        }

        [Fact]
        public async Task PrecioService_Combo_NoAcumulaPromociones()
        {
            var combo = new Producto { IdProducto = 13, Precio = 65m, IdTipoProducto = Producto.IdTipoCombo };
            var detalle = new DetallePedido { Cantidad = 1 };

            var clave = await Servicio("DOCENA").AplicarPrecioAsync(detalle, combo, Canales.Salon, DateTime.Today);

            Assert.Equal("COMBO", clave);
            Assert.Equal(65m, detalle.SubTotal);
        }

        [Fact]
        public async Task PrecioService_CodigoDesconocido_CaeEnPrecioNormal()
        {
            var producto = new Producto { IdProducto = 1, Precio = 18m, IdTipoProducto = 1 };
            var detalle = new DetallePedido { Cantidad = 1 };

            var clave = await Servicio("3X2").AplicarPrecioAsync(detalle, producto, Canales.Delivery, DateTime.Today);

            Assert.Equal("NORMAL", clave);
            Assert.Equal(18m, detalle.SubTotal);
        }
    }
}
