namespace AlitasGoWeb.Services.Dtos
{
    public record VentaPorCanal(string Canal, int Pedidos, decimal Total);
    public record SaborVendido(string Sabor, int Porciones);
    public record ProductoVendido(string Producto, int Cantidad, decimal Importe);
    public record VentaPorTipoPago(string TipoPago, int Pedidos, decimal Total);

    public class ReporteVentas
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public decimal TotalVendido { get; set; }
        public int PedidosEntregados { get; set; }
        public int PedidosAnulados { get; set; }
        public int PedidosEnCurso { get; set; }
        public decimal TicketPromedio { get; set; }
        public List<VentaPorCanal> PorCanal { get; set; } = new();
        public List<SaborVendido> Sabores { get; set; } = new();
        public List<ProductoVendido> Productos { get; set; } = new();
        public List<VentaPorTipoPago> PorTipoPago { get; set; } = new();
    }
}
