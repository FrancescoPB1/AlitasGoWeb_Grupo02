using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Promocion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPromocion { get; set; }
        public string? Nombre { get; set; }
        public int IdTipoPromocion { get; set; }
        public int IdCanalPromocion { get; set; }
        public int IdDia { get; set; }
        public bool Activo { get; set; }

        [ForeignKey("IdTipoPromocion")]
        public virtual TipoPromocion? TipoPromocion { get; set; }

        [ForeignKey("IdCanalPromocion")]
        public virtual CanalPromocion? CanalPromocion { get; set; }

        [ForeignKey("IdDia")]
        public virtual Dia? Dia { get; set; }

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    }
}
