using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }
        [StringLength(8)]
        public string DNI { get; set; } = null!;
        [StringLength(100)]
        public string Nombre { get; set; } = null!;
        [StringLength(100)]
        public string ApellidoPaterno { get; set; } = null!;
        [StringLength(100)]
        public string ApellidoMaterno { get; set; } = null!;
        public bool Activo { get; set; }

        // Navegación
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        
    }
}
