using System.Globalization;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;

namespace AlitasGoWeb.Tests.Unitarias
{
    public class ReglasDominioTests
    {
        // ------------------- Máquina de estados -------------------
        [Theory]
        [InlineData(Estados.Recibido, Estados.EnPreparacion, false, true)]
        [InlineData(Estados.EnPreparacion, Estados.Listo, true, true)]
        [InlineData(Estados.Listo, Estados.EnReparto, true, true)]
        [InlineData(Estados.Listo, Estados.EnReparto, false, false)]   // un pedido de salón no sale a reparto
        [InlineData(Estados.Listo, Estados.Servido, false, true)]
        [InlineData(Estados.Listo, Estados.Servido, true, false)]      // un delivery no se "sirve" en mesa
        [InlineData(Estados.EnReparto, Estados.Entregado, true, true)]
        [InlineData(Estados.Servido, Estados.Entregado, false, true)]
        [InlineData(Estados.Recibido, Estados.Listo, false, false)]    // no se salta estados
        [InlineData(Estados.Entregado, Estados.Anulado, false, false)] // final
        [InlineData(Estados.Anulado, Estados.Recibido, false, false)]  // final
        [InlineData(Estados.EnReparto, Estados.Anulado, true, true)]
        public void TransicionValida_RespetaElDiagramaDeEstados(int actual, int nuevo, bool esDelivery, bool esperado)
        {
            Assert.Equal(esperado, MaquinaEstadosPedido.TransicionValida(actual, nuevo, esDelivery));
        }

        [Theory]
        [InlineData(Estados.Recibido, true)]
        [InlineData(Estados.EnPreparacion, true)]
        [InlineData(Estados.Listo, false)]
        [InlineData(Estados.EnReparto, false)]
        public void DebeReponerStock_SoloAntesDeTerminarLaCocina(int estado, bool esperado)
        {
            Assert.Equal(esperado, MaquinaEstadosPedido.DebeReponerStock(estado));
        }

        [Fact]
        public void PuedeEditar_SoloEnRecibido()
        {
            Assert.True(MaquinaEstadosPedido.PuedeEditar(Estados.Recibido));
            Assert.False(MaquinaEstadosPedido.PuedeEditar(Estados.EnPreparacion));
        }

        // ------------------- Promociones: día de la semana -------------------
        // Regresión: antes se comparaba "miércoles"/"sábado" (con tilde) contra "Miercoles"/"Sabado".
        [Theory]
        [InlineData(2026, 9, 21, 1)]   // lunes
        [InlineData(2026, 9, 22, 2)]   // martes
        [InlineData(2026, 9, 23, 3)]   // miércoles
        [InlineData(2026, 9, 26, 6)]   // sábado
        [InlineData(2026, 9, 27, 7)]   // domingo
        public void IdDiaDe_DevuelveElIdDeLaTablaDias(int anio, int mes, int dia, int esperado)
        {
            Assert.Equal(esperado, PromocionService.IdDiaDe(new DateTime(anio, mes, dia)));
        }

        // ------------------- Pedido (Creador / Experto) -------------------
        [Fact]
        public void AgregarDetalle_ProductoSinSabor_DescartaElSabor()
        {
            var pedido = new Pedido();
            var papas = new Producto { IdProducto = 6, Precio = 8m, RequiereSabor = false };

            var detalle = pedido.AgregarDetalle(papas, 1, idSabor: 3);

            Assert.Null(detalle.IdSabor);
            Assert.Equal(8m, detalle.PrecioUnitario);
            Assert.Single(pedido.DetallePedidos);
        }

        [Fact]
        public void AgregarDetalle_CantidadCero_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() => new Pedido().AgregarDetalle(new Producto(), 0, null));
        }

        [Fact]
        public void CalcularTotal_SumaSubtotalesYCostoDeEnvio()
        {
            var pedido = new Pedido();
            pedido.DetallePedidos.Add(new DetallePedido { SubTotal = 30m });
            pedido.DetallePedidos.Add(new DetallePedido { SubTotal = 8m });

            Assert.Equal(43m, pedido.CalcularTotal(5m));
            Assert.Equal(43m, pedido.Total);
        }

        [Fact]
        public void Insumo_BajoMinimo_CuandoStockLlegaAlMinimo()
        {
            Assert.True(new Insumo { StockActual = 5, StockMinimo = 5 }.BajoMinimo);
            Assert.False(new Insumo { StockActual = 6, StockMinimo = 5 }.BajoMinimo);
            Assert.False(new Insumo { StockActual = 1 }.HayStock(1.5m));
        }

        // ------------------- IGV incluido y Singleton -------------------
        [Theory]
        [InlineData("118.00", "100.00", "18.00")]
        [InlineData("73.00", "61.86", "11.14")]
        [InlineData("25.50", "21.61", "3.89")]
        public void CalculadoraIgv_DesglosaPrecioConIgvIncluido(string total, string baseEsperada, string igvEsperado)
        {
            var d = new CalculadoraIgv(ConfiguracionSistema.Instancia).Desglosar(decimal.Parse(total, CultureInfo.InvariantCulture));

            Assert.Equal(decimal.Parse(baseEsperada, CultureInfo.InvariantCulture), d.BaseImponible);
            Assert.Equal(decimal.Parse(igvEsperado, CultureInfo.InvariantCulture), d.Igv);
            Assert.Equal(d.Total, d.BaseImponible + d.Igv);
        }

        [Fact]
        public void ConfiguracionSistema_EsUnaSolaInstancia()
        {
            Assert.Same(ConfiguracionSistema.Instancia, ConfiguracionSistema.Instancia);
            Assert.Equal(0.18m, ConfiguracionSistema.Instancia.Igv);
        }
    }
}
