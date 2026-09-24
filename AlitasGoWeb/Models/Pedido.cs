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

        [StringLength(256)]
        public string? UsuarioRegistro { get; set; }

        [StringLength(200)]
        public string? MotivoAnulacion { get; set; }

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

        [NotMapped]
        public bool EsDelivery => IdCanalAtencion == Canales.Delivery;

        // GRASP Creador: el Pedido contiene sus detalles y tiene los datos para crearlos.
        public DetallePedido AgregarDetalle(Producto producto, int cantidad, int? idSabor)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(cantidad));

            var detalle = new DetallePedido
            {
                IdProducto = producto.IdProducto,
                Producto = producto,
                IdSabor = producto.RequiereSabor ? idSabor : null,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio   // dato histórico: se congela el precio del día
            };
            DetallePedidos.Add(detalle);
            return detalle;
        }

        // GRASP Experto en información: el Pedido conoce todos sus detalles.
        public decimal CalcularTotal(decimal costoEnvio)
        {
            Total = DetallePedidos.Sum(d => d.SubTotal) + costoEnvio;
            return Total;
        }
    }
}
