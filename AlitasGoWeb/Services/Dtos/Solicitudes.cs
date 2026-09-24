namespace AlitasGoWeb.Services.Dtos
{
    public class LineaPedido
    {
        public int IdProducto { get; set; }
        public int? IdSabor { get; set; }
        public int Cantidad { get; set; }
    }

    public class SolicitudPedido
    {
        public int IdCliente { get; set; }
        public int IdCanalAtencion { get; set; }
        public int? IdNroMesa { get; set; }
        public int? IdZonaDelivery { get; set; }
        public string? Direccion { get; set; }
        public string? Referencia { get; set; }
        public string? Telefono { get; set; }
        public List<LineaPedido> Lineas { get; set; } = new();
    }

    public class SolicitudCobro
    {
        public int IdTipoPago { get; set; }
        public string TipoComprobante { get; set; } = Models.TiposComprobante.Boleta;
        public string? Ruc { get; set; }
        public string? RazonSocial { get; set; }
    }

    public class LineaCompra
    {
        public int IdInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }

    public record ConsumoInsumo(int IdInsumo, decimal Cantidad);

    public class SolicitudPromocion
    {
        public string Nombre { get; set; } = string.Empty;
        public int IdTipoPromocion { get; set; }
        public int IdCanalPromocion { get; set; }
        public int IdDia { get; set; }
        public List<int> IdsProducto { get; set; } = new();
    }
}
