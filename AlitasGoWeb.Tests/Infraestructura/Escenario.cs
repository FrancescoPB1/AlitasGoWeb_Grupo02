using AlitasGoWeb.Data;
using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Dtos;
using AlitasGoWeb.Services.Precios;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AlitasGoWeb.Tests.Infraestructura
{
    // Registra los avisos de tiempo real en lugar de enviarlos por SignalR.
    public class NotificadorFalso : INotificadorPedidos
    {
        public List<AvisoPedido> Avisos { get; } = new();
        public Task PedidoActualizadoAsync(AvisoPedido aviso)
        {
            Avisos.Add(aviso);
            return Task.CompletedTask;
        }
    }

    public class RelojFijo : IReloj
    {
        public RelojFijo(DateTime ahora) { Ahora = ahora; }
        public DateTime Ahora { get; set; }
    }

    // Fechas de referencia (setiembre de 2026)
    public static class Dias
    {
        public static readonly DateTime Lunes = new(2026, 9, 21, 13, 0, 0);
        public static readonly DateTime Martes = new(2026, 9, 22, 13, 0, 0);
        public static readonly DateTime Miercoles = new(2026, 9, 23, 13, 0, 0);
        public static readonly DateTime Jueves = new(2026, 9, 24, 13, 0, 0);
    }

    /// <summary>
    /// Prueba de integración: base SQLite en memoria creada desde el mismo modelo de EF Core
    /// (con sus semillas) y los servicios reales conectados como en Program.cs.
    /// </summary>
    public sealed class Escenario : IDisposable
    {
        public const string Usuario = "kevin@alitasgo.pe";
        public const string Admin = "rosa@alitasgo.pe";

        private readonly SqliteConnection _conexion;

        public AlitasGoDbContext Db { get; }
        public RelojFijo Reloj { get; }
        public NotificadorFalso Notificador { get; } = new();
        public IUnidadDeTrabajo Uow { get; }
        public AuditoriaService Auditoria { get; }
        public PromocionService Promociones { get; }
        public PrecioService Precios { get; }
        public InventarioService Inventario { get; }
        public CatalogoService Catalogo { get; }
        public PedidoService Pedidos { get; }
        public ReporteService Reportes { get; }

        public Escenario(DateTime? ahora = null)
        {
            _conexion = new SqliteConnection("DataSource=:memory:");
            _conexion.Open();

            Db = NuevoContexto();
            Db.Database.EnsureCreated();

            Reloj = new RelojFijo(ahora ?? Dias.Lunes);
            Uow = new UnidadDeTrabajo(Db);

            var pedidosRepo = new PedidoRepository(Db);
            var catalogoRepo = new CatalogoRepository(Db);
            var inventarioRepo = new InventarioRepository(Db);

            Auditoria = new AuditoriaService(new AuditoriaRepository(Db), Uow, Reloj);
            Promociones = new PromocionService(new PromocionRepository(Db), Auditoria, Uow);
            Precios = new PrecioService(
                new IEstrategiaPrecio[] { new PrecioNormal(), new PromocionDocena(), new Promocion2x1(), new PrecioCombo() },
                Promociones);
            Inventario = new InventarioService(inventarioRepo, catalogoRepo, Auditoria, Uow, Reloj);
            Catalogo = new CatalogoService(catalogoRepo, Auditoria, Uow, new MemoryCache(new MemoryCacheOptions()));
            Pedidos = new PedidoService(pedidosRepo, catalogoRepo, Precios, Inventario, Auditoria, Uow, Reloj,
                ConfiguracionSistema.Instancia, Notificador);
            Reportes = new ReporteService(pedidosRepo);
        }

        // Contexto nuevo sobre la misma base: sirve para verificar lo que realmente quedó guardado.
        public AlitasGoDbContext NuevoContexto() =>
            new(new DbContextOptionsBuilder<AlitasGoDbContext>().UseSqlite(_conexion).Options);

        public decimal Stock(int idInsumo)
        {
            using var ctx = NuevoContexto();
            return ctx.Insumos.Single(i => i.IdInsumo == idInsumo).StockActual;
        }

        public Pedido Leer(int idPedido)
        {
            using var ctx = NuevoContexto();
            return ctx.Pedidos
                .Include(p => p.DetallePedidos)
                .Include(p => p.PedidoDelivery)
                .Include(p => p.PedidoLocal)
                .Include(p => p.Pago)
                .Single(p => p.IdPedido == idPedido);
        }

        public static SolicitudPedido Salon(int mesa, params LineaPedido[] lineas) => new()
        {
            IdCliente = 1,
            IdCanalAtencion = Canales.Salon,
            IdNroMesa = mesa,
            Lineas = lineas.ToList()
        };

        public static SolicitudPedido Delivery(int zona, params LineaPedido[] lineas) => new()
        {
            IdCliente = 2,
            IdCanalAtencion = Canales.Delivery,
            IdZonaDelivery = zona,
            Direccion = "Av. España 1234",
            Referencia = "Portón azul, frente a la bodega",
            Telefono = "987654321",
            Lineas = lineas.ToList()
        };

        public static LineaPedido Linea(int idProducto, int cantidad, int? idSabor = null) =>
            new() { IdProducto = idProducto, Cantidad = cantidad, IdSabor = idSabor };

        public async Task<int> RegistrarAsync(SolicitudPedido s)
        {
            var r = await Pedidos.RegistrarAsync(s, Usuario);
            Assert.True(r.Exito, string.Join(" | ", r.Errores));
            return r.Valor;
        }

        public async Task LlevarAAsync(int idPedido, params int[] estados)
        {
            foreach (var e in estados)
            {
                var r = await Pedidos.CambiarEstadoAsync(idPedido, e, Usuario);
                Assert.True(r.Exito, string.Join(" | ", r.Errores));
            }
        }

        public void Dispose()
        {
            Db.Dispose();
            _conexion.Dispose();
        }
    }
}
