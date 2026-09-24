using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProducto { get; set; }

        public const int IdTipoCombo = 4;

        [Display(Name = "Categoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
        public int IdCategoriaProducto { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Tipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un tipo.")]
        public int IdTipoProducto { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(255, ErrorMessage = "Máximo {1} caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Precio (incluye IGV)")]
        [Range(0.10, 999.00, ErrorMessage = "Precio entre S/ 0.10 y S/ 999")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Display(Name = "¿Requiere sabor?")]
        public bool RequiereSabor { get; set; }

        [Display(Name = "Activo")]
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

        [NotMapped]
        public bool EsCombo => IdTipoProducto == IdTipoCombo;

    }
}
