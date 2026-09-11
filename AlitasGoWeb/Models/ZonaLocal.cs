using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class ZonaLocal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdZonaLocal { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
        public virtual ICollection<NroMesa> Mesas { get; set; } = new List<NroMesa>();
    }
}
