using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class ComboProducto
    {
        public int IdProductoCombo { get; set; }
        [ForeignKey(nameof(IdProductoCombo))]
        public virtual Producto? ProductoCombo { get; set; }

        public int IdProductoComponente { get; set; }
        [ForeignKey(nameof(IdProductoComponente))]
        public virtual Producto? ProductoComponente { get; set; }

        public int Cantidad { get; set; }

        public int? IdSabor { get; set; }
        [ForeignKey(nameof(IdSabor))]
        public virtual Sabor? Sabor { get; set; }
    }
}
