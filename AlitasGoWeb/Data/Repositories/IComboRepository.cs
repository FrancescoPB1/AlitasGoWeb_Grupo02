using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    // Productos que forman un combo (tabla ComboProductos).
    public interface IComboRepository
    {
        Task<List<ComboProducto>> ListarComponentesAsync(int idCombo);
        Task<ComboProducto?> ObtenerAsync(int idCombo, int idComponente);
        Task AgregarAsync(ComboProducto componente);
        void Eliminar(ComboProducto componente);
    }
}
