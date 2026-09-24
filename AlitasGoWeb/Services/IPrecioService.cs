using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface IPrecioService
    {
        // Devuelve la clave de la estrategia aplicada y deja SubTotal y Descuento calculados.
        Task<string> AplicarPrecioAsync(DetallePedido detalle, Producto producto, int idCanal, DateTime fecha);
    }
}
