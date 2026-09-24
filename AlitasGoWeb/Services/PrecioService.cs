using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Precios;

namespace AlitasGoWeb.Services
{
    // Contexto del patrón Strategy: elige la estrategia en tiempo de ejecución según
    // el tipo de producto, el día y el canal. Agregar una promoción nueva = una clase nueva.
    public class PrecioService : IPrecioService
    {
        private const string ClaveNormal = "NORMAL";
        private const string ClaveCombo = "COMBO";

        private readonly IEnumerable<IEstrategiaPrecio> _estrategias;
        private readonly IPromocionService _promociones;

        public PrecioService(IEnumerable<IEstrategiaPrecio> estrategias, IPromocionService promociones)
        {
            _estrategias = estrategias;
            _promociones = promociones;
        }

        public async Task<string> AplicarPrecioAsync(DetallePedido detalle, Producto producto, int idCanal, DateTime fecha)
        {
            string clave;
            if (producto.EsCombo)
                clave = ClaveCombo;
            else
                clave = await _promociones.ObtenerCodigoAplicableAsync(producto.IdProducto, idCanal, fecha) ?? ClaveNormal;

            var estrategia = _estrategias.FirstOrDefault(e => e.Clave == clave)
                          ?? _estrategias.First(e => e.Clave == ClaveNormal);

            detalle.PrecioUnitario = producto.Precio;   // siempre desde la BD, nunca desde el navegador
            detalle.SubTotal = estrategia.CalcularPrecio(detalle);
            detalle.Descuento = detalle.ImporteSinDescuento() - detalle.SubTotal;
            return estrategia.Clave;
        }
    }
}
