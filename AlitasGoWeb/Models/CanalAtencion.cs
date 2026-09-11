using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class CanalAtencion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCanalAtencion { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }

        // Navegación: Un canal de atención tiene muchos pedidos
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
