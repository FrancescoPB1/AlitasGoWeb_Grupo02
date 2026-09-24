using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Pago
    {
        public int IdPedido { get; set; }
        public int IdTipoPago { get; set; }

        public DateTime FechaPago { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Required, StringLength(10)]
        public string TipoComprobante { get; set; } = TiposComprobante.Boleta;

        [Required, StringLength(20)]
        public string NumeroComprobante { get; set; } = string.Empty;

        // DNI del cliente (boleta) o RUC de la empresa (factura)
        [StringLength(11)]
        public string? DocumentoCliente { get; set; }

        [StringLength(150)]
        public string? RazonSocial { get; set; }

        [StringLength(256)]
        public string? UsuarioCobro { get; set; }

        [ForeignKey("IdPedido")]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey("IdTipoPago")]
        public virtual TipoPago? TipoPago { get; set; }
    }
}
