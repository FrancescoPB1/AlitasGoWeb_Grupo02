using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class NroMesa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdNroMesa { get; set; }
        public int IdZonaLocal { get; set; }
        public int NumeroMesa { get; set; }
        public bool Activo { get; set; }

        [ForeignKey("IdZonaLocal")]
        public virtual ZonaLocal? ZonaLocal { get; set; }

        // Navegación
        public virtual ICollection<PedidoLocal> PedidosLocales { get; set; } = new List<PedidoLocal>();
    }
}
