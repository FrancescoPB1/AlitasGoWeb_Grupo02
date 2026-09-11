using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class TipoPago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoPago { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
