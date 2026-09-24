using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class DetallePedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetallePedido { get; set; }

        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int? IdSabor { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [ForeignKey("IdPedido")]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey("IdProducto")]
        public virtual Producto? Producto { get; set; }

        [ForeignKey("IdSabor")]
        public virtual Sabor? Sabor { get; set; }

        // GRASP Experto: conoce cantidad y precio.
        public decimal ImporteSinDescuento() => PrecioUnitario * Cantidad;
    }
}
