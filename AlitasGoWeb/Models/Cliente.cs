using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }

        [Display(Name = "DNI")]
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
        [StringLength(8)]
        public string DNI { get; set; } = null!;

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Apellido paterno")]
        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string ApellidoPaterno { get; set; } = null!;

        [Display(Name = "Apellido materno")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string ApellidoMaterno { get; set; } = null!;

        public bool Activo { get; set; }

        // Navegación
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {ApellidoPaterno}".Trim();
    }
}
