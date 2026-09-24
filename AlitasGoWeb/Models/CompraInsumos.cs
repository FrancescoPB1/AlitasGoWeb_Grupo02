using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class CompraInsumos
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(256)]
        public string? Usuario { get; set; }

        // Navegación
        public ICollection<DetalleCompraInsumo> Detalles { get; set; } = new List<DetalleCompraInsumo>();
    }
}
