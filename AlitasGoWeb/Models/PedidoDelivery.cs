using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class PedidoDelivery
    {
        [Key]
        public int IdPedido { get; set; }
        public int IdZonaDelivery { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        // "la casa del portón azul, frente a la bodega"
        [StringLength(200)]
        public string? Referencia { get; set; }

        [StringLength(15)]
        public string? Telefono { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoEnvio { get; set; }

        [ForeignKey("IdPedido")]
        public virtual Pedido? Pedido { get; set; }
        [ForeignKey("IdZonaDelivery")]
        public virtual ZonaDelivery? ZonaDelivery { get; set; }
    }
}
