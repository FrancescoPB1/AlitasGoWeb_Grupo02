using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Sabor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSabor { get; set; }

        [Required, StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public virtual ICollection<Receta> Recetas { get; set; } = new List<Receta>();
        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
        public virtual ICollection<ComboProducto> ComboProductos { get; set; } = new List<ComboProducto>();
    }
}
