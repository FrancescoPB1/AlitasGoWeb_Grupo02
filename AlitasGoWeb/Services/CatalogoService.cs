using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AlitasGoWeb.Services
{
    public class CatalogoService : ICatalogoService
    {
        // Driver de rendimiento: el catálogo se consulta en cada pedido de la hora punta.
        public const string ClaveProductos = "catalogo:productos-activos";
        public const string ClaveSabores = "catalogo:sabores-activos";
        private static readonly TimeSpan DuracionCache = TimeSpan.FromMinutes(10);

        private readonly ICatalogoRepository _repo;
        private readonly IAuditoriaService _auditoria;
        private readonly IUnidadDeTrabajo _uow;
        private readonly IMemoryCache _cache;

        public CatalogoService(ICatalogoRepository repo, IAuditoriaService auditoria, IUnidadDeTrabajo uow, IMemoryCache cache)
        {
            _repo = repo;
            _auditoria = auditoria;
            _uow = uow;
            _cache = cache;
        }

        // ---------------- Productos ----------------
        public Task<List<Producto>> ListarProductosAsync(bool soloActivos) => _repo.ListarProductosAsync(soloActivos);

        public async Task<List<Producto>> ListarProductosParaPedidoAsync() =>
            await _cache.GetOrCreateAsync(ClaveProductos, e =>
            {
                e.AbsoluteExpirationRelativeToNow = DuracionCache;
                return _repo.ListarProductosAsync(true);
            }) ?? new List<Producto>();

        public Task<Producto?> ObtenerProductoAsync(int idProducto) => _repo.ObtenerProductoAsync(idProducto);

        public async Task<Resultado> CrearProductoAsync(Producto producto, string usuario)
        {
            var error = await ValidarProductoAsync(producto);
            if (error != null) return Resultado.Error(error);

            producto.IdProducto = 0;
            producto.Nombre = producto.Nombre.Trim();
            producto.Descripcion = producto.Descripcion?.Trim() ?? string.Empty;
            producto.Activo = true;

            await _repo.AgregarProductoAsync(producto);
            await _auditoria.RegistrarAsync(usuario, "Crear producto", "Producto", null,
                $"{producto.Nombre} · S/ {producto.Precio:0.00}");
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok($"Producto «{producto.Nombre}» creado.");
        }

        public async Task<Resultado> ActualizarProductoAsync(Producto datos, string usuario)
        {
            var producto = await _repo.ObtenerProductoAsync(datos.IdProducto);
            if (producto == null) return Resultado.Error("El producto no existe.");

            var error = await ValidarProductoAsync(datos);
            if (error != null) return Resultado.Error(error);

            // Quién tocó un precio: queda en la bitácora con el valor anterior y el nuevo.
            if (producto.Precio != datos.Precio)
                await _auditoria.RegistrarAsync(usuario, "Cambiar precio", "Producto", producto.IdProducto.ToString(),
                    $"{producto.Nombre}: S/ {producto.Precio:0.00} → S/ {datos.Precio:0.00}");

            producto.Nombre = datos.Nombre.Trim();
            producto.Descripcion = datos.Descripcion?.Trim() ?? string.Empty;
            producto.IdCategoriaProducto = datos.IdCategoriaProducto;
            producto.IdTipoProducto = datos.IdTipoProducto;
            producto.Precio = datos.Precio;
            producto.RequiereSabor = datos.RequiereSabor;
            producto.Activo = datos.Activo;

            await _auditoria.RegistrarAsync(usuario, "Editar producto", "Producto", producto.IdProducto.ToString(), producto.Nombre);
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok("Producto actualizado.");
        }

        public async Task<Resultado> DesactivarProductoAsync(int idProducto, string usuario)
        {
            var producto = await _repo.ObtenerProductoAsync(idProducto);
            if (producto == null) return Resultado.Error("El producto no existe.");

            producto.Activo = false;   // borrado lógico: los pedidos antiguos lo siguen referenciando
            await _auditoria.RegistrarAsync(usuario, "Desactivar producto", "Producto", idProducto.ToString(), producto.Nombre);
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok($"Producto «{producto.Nombre}» desactivado.");
        }

        private async Task<string?> ValidarProductoAsync(Producto p)
        {
            if (string.IsNullOrWhiteSpace(p.Nombre)) return "El nombre es obligatorio.";
            if (p.Precio < 0.10m || p.Precio > 999m) return "Precio entre S/ 0.10 y S/ 999.";
            if (await _repo.ObtenerCategoriaAsync(p.IdCategoriaProducto) == null) return "Selecciona una categoría válida.";
            if ((await _repo.ListarTiposProductoAsync()).All(t => t.IdTipoProducto != p.IdTipoProducto)) return "Selecciona un tipo válido.";
            return null;
        }

        // ---------------- Categorías ----------------
        public Task<List<CategoriaProducto>> ListarCategoriasAsync(bool soloActivas) => _repo.ListarCategoriasAsync(soloActivas);

        public Task<CategoriaProducto?> ObtenerCategoriaAsync(int idCategoria) => _repo.ObtenerCategoriaAsync(idCategoria);

        public async Task<bool> NombreCategoriaDisponibleAsync(string nombre, int excluirId) =>
            !await _repo.ExisteCategoriaAsync((nombre ?? string.Empty).Trim(), excluirId);

        public async Task<Resultado> CrearCategoriaAsync(CategoriaProducto categoria, string usuario)
        {
            categoria.Nombre = categoria.Nombre?.Trim() ?? string.Empty;
            if (!await NombreCategoriaDisponibleAsync(categoria.Nombre, 0))
                return Resultado.Error($"La categoría {categoria.Nombre} ya existe.");

            categoria.IdCategoriaProducto = 0;
            categoria.Activo = true;
            await _repo.AgregarCategoriaAsync(categoria);
            await _auditoria.RegistrarAsync(usuario, "Crear categoría", "CategoriaProducto", null, categoria.Nombre);
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok($"Categoría «{categoria.Nombre}» creada.");
        }

        public async Task<Resultado> ActualizarCategoriaAsync(CategoriaProducto datos, string usuario)
        {
            var categoria = await _repo.ObtenerCategoriaAsync(datos.IdCategoriaProducto);
            if (categoria == null) return Resultado.Error("La categoría no existe.");

            var nombre = datos.Nombre?.Trim() ?? string.Empty;
            if (!await NombreCategoriaDisponibleAsync(nombre, categoria.IdCategoriaProducto))
                return Resultado.Error($"La categoría {nombre} ya existe.");
            if (categoria.Activo && !datos.Activo && await TieneProductosActivosAsync(categoria.IdCategoriaProducto))
                return Resultado.Error("No se puede desactivar: la categoría tiene productos activos.");

            categoria.Nombre = nombre;
            categoria.Activo = datos.Activo;
            await _auditoria.RegistrarAsync(usuario, "Editar categoría", "CategoriaProducto", categoria.IdCategoriaProducto.ToString(), nombre);
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok("Categoría actualizada.");
        }

        public async Task<Resultado> DesactivarCategoriaAsync(int idCategoria, string usuario)
        {
            var categoria = await _repo.ObtenerCategoriaAsync(idCategoria);
            if (categoria == null) return Resultado.Error("La categoría no existe.");

            // Regla de negocio (Semana 3): no se elimina una categoría con productos.
            if (await TieneProductosActivosAsync(idCategoria))
                return Resultado.Error("No se puede eliminar: la categoría tiene productos activos.");

            categoria.Activo = false;
            await _auditoria.RegistrarAsync(usuario, "Desactivar categoría", "CategoriaProducto", idCategoria.ToString(), categoria.Nombre);
            await _uow.GuardarCambiosAsync();
            InvalidarCache();
            return Resultado.Ok($"Categoría «{categoria.Nombre}» desactivada.");
        }

        private async Task<bool> TieneProductosActivosAsync(int idCategoria) =>
            (await _repo.ListarProductosAsync(true)).Any(p => p.IdCategoriaProducto == idCategoria);

        // ---------------- Tablas de apoyo ----------------
        public Task<List<TipoProducto>> ListarTiposProductoAsync() => _repo.ListarTiposProductoAsync();

        public async Task<List<Sabor>> ListarSaboresAsync() =>
            await _cache.GetOrCreateAsync(ClaveSabores, e =>
            {
                e.AbsoluteExpirationRelativeToNow = DuracionCache;
                return _repo.ListarSaboresAsync(true);
            }) ?? new List<Sabor>();

        public Task<List<CanalAtencion>> ListarCanalesAsync() => _repo.ListarCanalesAsync();
        public Task<List<NroMesa>> ListarMesasAsync() => _repo.ListarMesasAsync();
        public Task<List<ZonaDelivery>> ListarZonasDeliveryAsync() => _repo.ListarZonasDeliveryAsync();
        public Task<List<TipoPago>> ListarTiposPagoAsync() => _repo.ListarTiposPagoAsync();

        // ---------------- Clientes ----------------
        public Task<List<Cliente>> ListarClientesAsync() => _repo.ListarClientesAsync();

        public async Task<Resultado<int>> CrearClienteAsync(Cliente cliente)
        {
            cliente.DNI = cliente.DNI?.Trim() ?? string.Empty;
            if (await _repo.ExisteDniAsync(cliente.DNI))
                return Resultado<int>.Error("Ya existe un cliente con ese DNI.");

            cliente.IdCliente = 0;
            cliente.Nombre = cliente.Nombre.Trim();
            cliente.ApellidoPaterno = cliente.ApellidoPaterno.Trim();
            cliente.ApellidoMaterno = cliente.ApellidoMaterno?.Trim() ?? string.Empty;
            cliente.Activo = true;
            await _repo.AgregarClienteAsync(cliente);
            await _uow.GuardarCambiosAsync();
            return Resultado<int>.Ok(cliente.IdCliente, $"Cliente {cliente.Nombre} {cliente.ApellidoPaterno} registrado.");
        }

        private void InvalidarCache()
        {
            _cache.Remove(ClaveProductos);
            _cache.Remove(ClaveSabores);
        }
    }
}
