using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface ICatalogoRepository
    {
        // Productos
        Task<List<Producto>> ListarProductosAsync(bool soloActivos);
        Task<Producto?> ObtenerProductoAsync(int idProducto);
        Task<List<Producto>> ObtenerProductosAsync(IEnumerable<int> ids);
        Task AgregarProductoAsync(Producto producto);

        // Categorías
        Task<List<CategoriaProducto>> ListarCategoriasAsync(bool soloActivas);
        Task<CategoriaProducto?> ObtenerCategoriaAsync(int idCategoria);
        Task<bool> ExisteCategoriaAsync(string nombre, int excluirId);
        Task AgregarCategoriaAsync(CategoriaProducto categoria);

        // Tablas de apoyo
        Task<List<TipoProducto>> ListarTiposProductoAsync();
        Task<List<Sabor>> ListarSaboresAsync(bool soloActivos);
        Task<Sabor?> ObtenerSaborAsync(int idSabor);
        Task<List<CanalAtencion>> ListarCanalesAsync();
        Task<List<NroMesa>> ListarMesasAsync();
        Task<NroMesa?> ObtenerMesaAsync(int idNroMesa);
        Task<List<ZonaDelivery>> ListarZonasDeliveryAsync();
        Task<ZonaDelivery?> ObtenerZonaDeliveryAsync(int idZona);
        Task<List<TipoPago>> ListarTiposPagoAsync();
        Task<TipoPago?> ObtenerTipoPagoAsync(int idTipoPago);

        // Clientes
        Task<List<Cliente>> ListarClientesAsync();
        Task<Cliente?> ObtenerClienteAsync(int idCliente);
        Task<bool> ExisteDniAsync(string dni);
        Task AgregarClienteAsync(Cliente cliente);
    }
}
