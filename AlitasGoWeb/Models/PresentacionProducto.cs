using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class PresentacionProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPresentacion { get; set; }

        [Display(Name = "Producto")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un producto.")]
        public int IdProducto { get; set; }

        [Display(Name = "Nombre de la presentación")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "Máximo {1} caracteres.")]
        public string NombrePresentacion { get; set; } = string.Empty;

        [Display(Name = "Cantidad")]
        [Range(0.01, 9999, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadUnidad { get; set; }

        [Display(Name = "Precio")]
        [Range(0.01, 99999, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [ForeignKey(nameof(IdProducto))]
        public virtual Producto? Producto { get; set; }
    }
}
