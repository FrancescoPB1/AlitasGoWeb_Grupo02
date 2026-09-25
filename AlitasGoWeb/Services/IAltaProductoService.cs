using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // Alta de un producto junto con lo que lo compone (su receta de insumos).
    public interface IAltaProductoService
    {
        Task<Resultado> CrearConRecetaAsync(Producto producto, IList<LineaIngrediente> ingredientes, string usuario);
    }
}
