using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class CategoriaProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategoriaProducto { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        // Navegación: Una categoría tiene muchos productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
