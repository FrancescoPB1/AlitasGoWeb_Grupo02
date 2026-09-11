using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class EstadoPedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEstadoPedido { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }

        // Navegación
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
