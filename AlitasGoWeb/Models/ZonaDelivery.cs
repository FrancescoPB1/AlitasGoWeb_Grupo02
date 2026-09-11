using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class ZonaDelivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdZonaDelivery { get; set; }
        public string? NombreZona { get; set; }
        [Column(TypeName ="Decimal(10,2)")]
        public decimal CostoDelivery { get; set; }
        public bool Activo { get; set; }
        public virtual ICollection<PedidoDelivery> PedidosDelivery { get; set; } = new List<PedidoDelivery>();

    }
}
