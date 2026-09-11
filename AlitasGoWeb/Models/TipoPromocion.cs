using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class TipoPromocion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoPromocion { get; set; }
        public string? Nombre { get; set; } = null!;
        [Required, StringLength(30)]
        public string Codigo { get; set; } = string.Empty;   
        public bool Activo { get; set; }
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
