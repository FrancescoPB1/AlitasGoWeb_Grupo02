using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services.Precios
{
    public interface IEstrategiaPrecio
    {
        string Clave { get; }
        decimal CalcularPrecio(DetallePedido detalle);
    }
}
