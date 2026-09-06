using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProducto { get; set; }

        [Display(Name = "Categoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
        public int IdCategoriaProducto { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Tipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un tipo.")]
        public int IdTipoProducto { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(255, ErrorMessage = "Máximo {1} caracteres.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [ForeignKey(nameof(IdCategoriaProducto))]
        public virtual CategoriaProducto? CategoriaProducto { get; set; }

        [ForeignKey(nameof(IdTipoProducto))]
        public virtual TipoProducto? TipoProducto { get; set; }

        public virtual ICollection<PresentacionProducto> PrepresentacionProducto { get; set; } = new List<PresentacionProducto>();
    }
}
