using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class PedidoLocal
    {
        [Key]
        public int IdPedido { get; set; }
        public int IdNroMesa { get; set; }
        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }
        [ForeignKey("IdNroMesa")]
        public NroMesa? NroMesa { get; set; }
    }
}
