using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class CanalPromocion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCanalPromocion { get; set; }
        public string? Nombre { get; set; }
        public bool Activo { get; set; }
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
