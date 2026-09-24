using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Insumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdInsumo { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(120, ErrorMessage = "Máximo {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Unidad de medida")]
        [Required(ErrorMessage = "La unidad es obligatoria."), StringLength(20, ErrorMessage = "Máximo {1} caracteres.")]
        public string UnidadMedida { get; set; } = string.Empty;

        [Display(Name = "Stock actual")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockActual { get; set; }

        [Display(Name = "Stock mínimo")]
        [Range(0, 999999, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }

        public virtual ICollection<Receta> ProductoInsumos { get; set; } = new List<Receta>();
        public virtual ICollection<DetalleCompraInsumo> DetallesCompra { get; set; } = new List<DetalleCompraInsumo>();
        public virtual ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();

        // GRASP Experto: el insumo conoce su stock.
        public bool HayStock(decimal cantidad) => StockActual >= cantidad;

        [NotMapped]
        public bool BajoMinimo => StockActual <= StockMinimo;
    }
}
