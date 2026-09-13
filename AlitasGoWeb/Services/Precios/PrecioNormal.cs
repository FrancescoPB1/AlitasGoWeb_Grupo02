using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services.Precios
{
    public class PrecioNormal:IEstrategiaPrecio
    {
        public string Clave => "NORMAL";

        public decimal CalcularPrecio(DetallePedido detalle)
        {
            return detalle.PrecioUnitario * detalle.Cantidad;
        }
    }
}
