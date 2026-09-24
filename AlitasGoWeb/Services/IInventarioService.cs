using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public interface IInventarioService
    {
        Task<List<ConsumoInsumo>> CalcularConsumoAsync(IEnumerable<DetallePedido> detalles);
        Task<List<string>> ValidarStockAsync(IEnumerable<ConsumoInsumo> consumo);
        Task DescontarAsync(IEnumerable<ConsumoInsumo> consumo, string motivo, Pedido? pedido, string usuario);
        Task ReponerAsync(IEnumerable<ConsumoInsumo> consumo, string motivo, Pedido? pedido, string usuario);

        Task<List<Insumo>> ListarInsumosAsync();
        Task<Insumo?> ObtenerInsumoAsync(int idInsumo);
        Task<Resultado> CrearInsumoAsync(Insumo insumo, string usuario);
        Task<Resultado> EditarInsumoAsync(Insumo insumo, string usuario);
        Task<Resultado> RegistrarCompraAsync(List<LineaCompra> lineas, string usuario);
        Task<List<MovimientoInventario>> ListarMovimientosAsync(int? idInsumo);

        Task<List<Receta>> ListarRecetaAsync(int idProducto);
        Task<Resultado> AgregarLineaRecetaAsync(int idProducto, int idInsumo, int? idSabor, decimal cantidad, string usuario);
        Task<Resultado> QuitarLineaRecetaAsync(int idReceta, string usuario);
    }
}
