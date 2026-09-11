using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCategoriaProducto { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int IdTipoProducto { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public bool RequiereSabor { get; set; }

        public bool Activo { get; set; }

        [ForeignKey(nameof(IdCategoriaProducto))]
        public virtual CategoriaProducto? CategoriaProducto { get; set; }

        [ForeignKey(nameof(IdTipoProducto))]
        public virtual TipoProducto? TipoProducto { get; set; }

        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
        public virtual ICollection<Receta> ProductoInsumos { get; set; } = new List<Receta>();
        public virtual ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();

        // Autorreferencia para combos
        public virtual ICollection<ComboProducto> Componentes { get; set; } = new List<ComboProducto>();
        public virtual ICollection<ComboProducto> ComponenteDe { get; set; } = new List<ComboProducto>();

    }
}
