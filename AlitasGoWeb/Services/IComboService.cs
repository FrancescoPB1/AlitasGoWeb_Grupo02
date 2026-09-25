using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // Arma los combos: qué productos los forman. El stock que consume un combo es la suma
    // de las recetas de sus productos (InventarioService.CalcularConsumoAsync ya lo hace).
    public interface IComboService
    {
        Task<List<ComboProducto>> ListarComponentesAsync(int idCombo);
        Task<List<string>> ValidarComponentesAsync(int idCombo, IList<LineaComponente> lineas);
        Task<Resultado> AgregarComponenteAsync(int idCombo, LineaComponente linea, string usuario);
        Task<Resultado> QuitarComponenteAsync(int idCombo, int idComponente, string usuario);
    }
}
