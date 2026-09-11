using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class PedidoDelivery
    {
        [Key]
        public int IdPedido { get; set; }
        public int IdZonaDelivery { get; set; }
        
        public string? Direccion { get; set; }
        
        [ForeignKey("IdPedido")]
        public virtual Pedido? Pedido { get; set; }
        [ForeignKey("IdZonaDelivery")]
        public virtual ZonaDelivery? ZonaDelivery { get; set; }
        

    }
}
