using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class TipoProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoProducto { get; set; }
        [Required]
        [StringLength(10)]
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
