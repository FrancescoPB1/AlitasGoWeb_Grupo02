using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class ProductoInsumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProductoInsumo { get; set; }
        public int IdProducto { get; set; }
        public int IdInsumo { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadUsada { get; set; }
        [Required]
        [StringLength(20)]
        public string UnidadMedida { get; set; } = string.Empty;
        [ForeignKey("IdProducto")]
        public virtual Producto? Producto { get; set; }
        [ForeignKey("IdInsumo")]
        public virtual Insumo? Insumo { get; set; }
    }
}
