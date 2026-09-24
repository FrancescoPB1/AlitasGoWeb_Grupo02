using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IInventarioRepository
    {
        Task<List<Insumo>> ListarInsumosAsync();
        Task<Insumo?> ObtenerInsumoAsync(int idInsumo);
        Task<List<Insumo>> ObtenerInsumosAsync(IEnumerable<int> ids);
        Task<bool> ExisteInsumoAsync(string nombre, int excluirId);
        Task AgregarInsumoAsync(Insumo insumo);

        Task<List<Receta>> ObtenerRecetasAsync(IEnumerable<int> idsProducto);
        Task<List<Receta>> ListarRecetaDeProductoAsync(int idProducto);
        Task<Receta?> ObtenerRecetaAsync(int idReceta);
        Task AgregarRecetaAsync(Receta receta);
        void EliminarReceta(Receta receta);

        Task<List<ComboProducto>> ObtenerComponentesAsync(IEnumerable<int> idsCombo);

        Task AgregarCompraAsync(CompraInsumos compra);
        Task AgregarMovimientoAsync(MovimientoInventario movimiento);
        Task<List<MovimientoInventario>> ListarMovimientosAsync(int? idInsumo, int maximo);
    }
}
