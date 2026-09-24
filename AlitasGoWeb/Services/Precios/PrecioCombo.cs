using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services.Precios
{
    // Un combo ya viene armado a un precio pensado: se cobra su precio y no acumula otras promociones.
    public class PrecioCombo : IEstrategiaPrecio
    {
        public string Clave => "COMBO";

        public decimal CalcularPrecio(DetallePedido detalle)
        {
            return detalle.PrecioUnitario * detalle.Cantidad;
        }
    }
}
