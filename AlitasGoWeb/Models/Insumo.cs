using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Insumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdInsumo { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string UnidadMedida { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockActual { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }
        public bool Activo { get; set; }
    }
}
