using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public CatalogoRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<List<Producto>> ListarProductosAsync(bool soloActivos)
        {
            var q = _ctx.Productos.AsNoTracking()
                .Include(p => p.CategoriaProducto)
                .Include(p => p.TipoProducto)
                .AsQueryable();
            if (soloActivos) q = q.Where(p => p.Activo);
            return q.OrderBy(p => p.IdCategoriaProducto).ThenBy(p => p.Nombre).ToListAsync();
        }

        public Task<Producto?> ObtenerProductoAsync(int idProducto) =>
            _ctx.Productos.FirstOrDefaultAsync(p => p.IdProducto == idProducto);

        public Task<List<Producto>> ObtenerProductosAsync(IEnumerable<int> ids)
        {
            var lista = ids.Distinct().ToList();
            return _ctx.Productos.Where(p => lista.Contains(p.IdProducto)).ToListAsync();
        }

        public async Task AgregarProductoAsync(Producto producto) => await _ctx.Productos.AddAsync(producto);

        public Task<List<CategoriaProducto>> ListarCategoriasAsync(bool soloActivas)
        {
            var q = _ctx.CategoriasProducto.AsNoTracking().AsQueryable();
            if (soloActivas) q = q.Where(c => c.Activo);
            return q.OrderBy(c => c.Nombre).ToListAsync();
        }

        public Task<CategoriaProducto?> ObtenerCategoriaAsync(int idCategoria) =>
            _ctx.CategoriasProducto.FirstOrDefaultAsync(c => c.IdCategoriaProducto == idCategoria);

        public Task<bool> ExisteCategoriaAsync(string nombre, int excluirId) =>
            _ctx.CategoriasProducto.AnyAsync(c => c.Nombre == nombre && c.IdCategoriaProducto != excluirId);

        public async Task AgregarCategoriaAsync(CategoriaProducto categoria) => await _ctx.CategoriasProducto.AddAsync(categoria);

        public Task<List<TipoProducto>> ListarTiposProductoAsync() =>
            _ctx.TiposProducto.AsNoTracking().Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();

        public Task<List<Sabor>> ListarSaboresAsync(bool soloActivos)
        {
            var q = _ctx.Sabores.AsNoTracking().AsQueryable();
            if (soloActivos) q = q.Where(s => s.Activo);
            return q.OrderBy(s => s.Nombre).ToListAsync();
        }

        public Task<Sabor?> ObtenerSaborAsync(int idSabor) =>
            _ctx.Sabores.AsNoTracking().FirstOrDefaultAsync(s => s.IdSabor == idSabor);

        public Task<List<CanalAtencion>> ListarCanalesAsync() =>
            _ctx.CanalesAtencion.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.IdCanalAtencion).ToListAsync();

        public Task<List<NroMesa>> ListarMesasAsync() =>
            _ctx.Mesas.AsNoTracking().Include(m => m.ZonaLocal)
                .Where(m => m.Activo).OrderBy(m => m.NumeroMesa).ToListAsync();

        public Task<NroMesa?> ObtenerMesaAsync(int idNroMesa) =>
            _ctx.Mesas.AsNoTracking().FirstOrDefaultAsync(m => m.IdNroMesa == idNroMesa);

        public Task<List<ZonaDelivery>> ListarZonasDeliveryAsync() =>
            _ctx.ZonasDelivery.AsNoTracking().Where(z => z.Activo).OrderBy(z => z.IdZonaDelivery).ToListAsync();

        public Task<ZonaDelivery?> ObtenerZonaDeliveryAsync(int idZona) =>
            _ctx.ZonasDelivery.AsNoTracking().FirstOrDefaultAsync(z => z.IdZonaDelivery == idZona);

        public Task<List<TipoPago>> ListarTiposPagoAsync() =>
            _ctx.TiposPago.AsNoTracking().Where(t => t.Activo).OrderBy(t => t.IdTipoPago).ToListAsync();

        public Task<TipoPago?> ObtenerTipoPagoAsync(int idTipoPago) =>
            _ctx.TiposPago.AsNoTracking().FirstOrDefaultAsync(t => t.IdTipoPago == idTipoPago);

        public Task<List<Cliente>> ListarClientesAsync() =>
            _ctx.Clientes.AsNoTracking().Where(c => c.Activo)
                .OrderBy(c => c.Nombre).ThenBy(c => c.ApellidoPaterno).ToListAsync();

        public Task<Cliente?> ObtenerClienteAsync(int idCliente) =>
            _ctx.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.IdCliente == idCliente);

        public Task<bool> ExisteDniAsync(string dni) => _ctx.Clientes.AnyAsync(c => c.DNI == dni);

        public async Task AgregarClienteAsync(Cliente cliente) => await _ctx.Clientes.AddAsync(cliente);
    }
}
