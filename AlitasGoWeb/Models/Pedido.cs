using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPedido { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int IdCliente { get; set; }
        public int IdCanalAtencion { get; set; }
        public int IdEstadoPedido { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdCanalAtencion")]
        public virtual CanalAtencion? CanalAtencion { get; set; }

        [ForeignKey("IdEstadoPedido")]
        public virtual EstadoPedido? EstadoPedido { get; set; }

        public virtual PedidoDelivery? PedidoDelivery { get; set; }
        public virtual PedidoLocal? PedidoLocal { get; set; }
        public virtual Pago? Pago { get; set; }

        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    }
}
