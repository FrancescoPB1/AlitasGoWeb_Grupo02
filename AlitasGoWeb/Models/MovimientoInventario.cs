using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    // Cada entrada (compra, anulación) o salida (pedido) de un insumo queda registrada.
    public class MovimientoInventario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMovimientoInventario { get; set; }

        public int IdInsumo { get; set; }
        public int? IdPedido { get; set; }

        public DateTime Fecha { get; set; }

        [Required, StringLength(10)]
        public string Tipo { get; set; } = TiposMovimiento.Salida;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        [Required, StringLength(200)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(256)]
        public string? Usuario { get; set; }

        [ForeignKey(nameof(IdInsumo))]
        public virtual Insumo? Insumo { get; set; }

        [ForeignKey(nameof(IdPedido))]
        public virtual Pedido? Pedido { get; set; }
    }
}
