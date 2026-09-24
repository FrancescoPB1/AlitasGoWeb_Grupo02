using AlitasGoWeb.Models;

namespace AlitasGoWeb.Services
{
    public interface ICatalogoService
    {
        Task<List<Producto>> ListarProductosAsync(bool soloActivos);
        Task<List<Producto>> ListarProductosParaPedidoAsync();   // en caché
        Task<Producto?> ObtenerProductoAsync(int idProducto);
        Task<Resultado> CrearProductoAsync(Producto producto, string usuario);
        Task<Resultado> ActualizarProductoAsync(Producto producto, string usuario);
        Task<Resultado> DesactivarProductoAsync(int idProducto, string usuario);

        Task<List<CategoriaProducto>> ListarCategoriasAsync(bool soloActivas);
        Task<CategoriaProducto?> ObtenerCategoriaAsync(int idCategoria);
        Task<bool> NombreCategoriaDisponibleAsync(string nombre, int excluirId);
        Task<Resultado> CrearCategoriaAsync(CategoriaProducto categoria, string usuario);
        Task<Resultado> ActualizarCategoriaAsync(CategoriaProducto categoria, string usuario);
        Task<Resultado> DesactivarCategoriaAsync(int idCategoria, string usuario);

        Task<List<TipoProducto>> ListarTiposProductoAsync();
        Task<List<Sabor>> ListarSaboresAsync();                  // en caché
        Task<List<CanalAtencion>> ListarCanalesAsync();
        Task<List<NroMesa>> ListarMesasAsync();
        Task<List<ZonaDelivery>> ListarZonasDeliveryAsync();
        Task<List<TipoPago>> ListarTiposPagoAsync();

        Task<List<Cliente>> ListarClientesAsync();
        Task<Resultado<int>> CrearClienteAsync(Cliente cliente);
    }
}
