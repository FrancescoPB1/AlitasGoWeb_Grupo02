using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services.Precios
{
    public class PromocionDocena:IEstrategiaPrecio
    {
        public string Clave => "DOCENA";
        public decimal CalcularPrecio(DetallePedido detalle)
        {
            var subtotal = detalle.PrecioUnitario * detalle.Cantidad;
            return subtotal * 0.85m;
        }
    
    }
}
