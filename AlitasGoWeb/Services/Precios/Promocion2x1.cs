using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services.Precios
{
    public class Promocion2x1:IEstrategiaPrecio
    {
        public string Clave => "2X1";

        
        public decimal CalcularPrecio(DetallePedido detalle)
        {
            int gratis = detalle.Cantidad / 2;
            int pagadas = detalle.Cantidad - gratis;
            return pagadas * detalle.PrecioUnitario;
        }
    }
}
