using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Pago
    {
        public int IdPedido { get; set; }
        public int IdTipoPago { get; set; }


        public DateTime FechaPago { get; set; }
        [ForeignKey("IdPedido")]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey("IdTipoPago")]
        public virtual TipoPago? TipoPago { get; set; }
        
    }
}
