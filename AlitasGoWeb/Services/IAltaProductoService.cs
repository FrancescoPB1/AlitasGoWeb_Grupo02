using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // Alta de un producto junto con lo que lo compone: su receta de insumos, o sus productos si es un combo.
    public interface IAltaProductoService
    {
        Task<Resultado> CrearConRecetaAsync(Producto producto, IList<LineaIngrediente> ingredientes, string usuario);
        Task<Resultado> CrearComboAsync(Producto combo, IList<LineaComponente> componentes, string usuario);
    }
}
