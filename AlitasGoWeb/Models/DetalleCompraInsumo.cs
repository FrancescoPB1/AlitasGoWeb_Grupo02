using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class DetalleCompraInsumo
    {
        public int Id { get; set; }

        public int CompraId { get; set; }
        public CompraInsumos CompraInsumos { get; set; } = null!;

        public int InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal ;
    }
}
