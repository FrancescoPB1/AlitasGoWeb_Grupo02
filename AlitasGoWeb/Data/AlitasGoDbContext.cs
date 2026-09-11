using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data
{
    public class AlitasGoDbContext : DbContext
    {
        public AlitasGoDbContext(DbContextOptions<AlitasGoDbContext> options) : base(options)
        {
        }
        // Catálogos
        public DbSet<Dia> Dias => Set<Dia>();
        public DbSet<CanalAtencion> CanalesAtencion => Set<CanalAtencion>();
        public DbSet<CanalPromocion> CanalesPromocion => Set<CanalPromocion>();
        public DbSet<EstadoPedido> EstadosPedido => Set<EstadoPedido>();
        public DbSet<TipoPago> TiposPago => Set<TipoPago>();
        public DbSet<TipoProducto> TiposProducto => Set<TipoProducto>();
        public DbSet<TipoPromocion> TiposPromocion => Set<TipoPromocion>();
        public DbSet<ZonaLocal> ZonasLocal => Set<ZonaLocal>();
        public DbSet<ZonaDelivery> ZonasDelivery => Set<ZonaDelivery>();
        public DbSet<NroMesa> Mesas => Set<NroMesa>();
        public DbSet<CategoriaProducto> CategoriasProducto => Set<CategoriaProducto>();

        // Productos y sabores
        public DbSet<Sabor> Sabores => Set<Sabor>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<ComboProducto> ComboProductos => Set<ComboProducto>();
        public DbSet<Receta> Recetas => Set<Receta>();

        // Promociones
        public DbSet<Promocion> Promociones => Set<Promocion>();

        // Clientes
        public DbSet<Cliente> Clientes => Set<Cliente>();

        // Insumos
        public DbSet<Insumo> Insumos => Set<Insumo>();
        public DbSet<CompraInsumos> ComprasInsumos => Set<CompraInsumos>();
        public DbSet<DetalleCompraInsumo> DetallesCompraInsumo => Set<DetalleCompraInsumo>();

        // Pedidos
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
        public DbSet<PedidoLocal> PedidosLocal => Set<PedidoLocal>();
        public DbSet<PedidoDelivery> PedidosDelivery => Set<PedidoDelivery>();
        public DbSet<Pago> Pagos => Set<Pago>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // ---------- ComboProducto: clave compuesta ----------
            mb.Entity<ComboProducto>(e =>
            {
                e.HasKey(c => new { c.IdProductoCombo, c.IdProductoComponente });

                e.HasOne(c => c.ProductoCombo)
                 .WithMany(p => p.Componentes)
                 .HasForeignKey(c => c.IdProductoCombo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.ProductoComponente)
                 .WithMany(p => p.ComponenteDe)
                 .HasForeignKey(c => c.IdProductoComponente)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.Sabor)
                 .WithMany(s => s.ComboProductos)
                 .HasForeignKey(c => c.IdSabor)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- PedidoLocal / PedidoDelivery / Pago: 1:1 ----------
            mb.Entity<PedidoLocal>()
              .HasOne(p => p.Pedido)
              .WithOne(p => p.PedidoLocal!)
              .HasForeignKey<PedidoLocal>(p => p.IdPedido)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<PedidoDelivery>()
              .HasOne(p => p.Pedido)
              .WithOne(p => p.PedidoDelivery!)
              .HasForeignKey<PedidoDelivery>(p => p.IdPedido)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Pago>(e =>
            {
                e.HasKey(p => p.IdPedido);
                e.HasOne(p => p.Pedido)
              .WithOne(p => p.Pago!)
              .HasForeignKey<Pago>(p => p.IdPedido)
              .OnDelete(DeleteBehavior.Cascade);
            });
              

            // ---------- Promocion N:M con Producto ----------
            mb.Entity<Promocion>()
              .HasMany(p => p.Productos)
              .WithMany(pr => pr.Promociones)
              .UsingEntity(j => j.ToTable("PromocionProducto"));

            // ---------- Receta ----------
            mb.Entity<Receta>(e =>
            {
                e.HasOne(r => r.Producto)
                 .WithMany(p => p.ProductoInsumos)
                 .HasForeignKey(r => r.IdProducto)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Insumo)
                 .WithMany(i => i.ProductoInsumos)
                 .HasForeignKey(r => r.IdInsumo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Sabor)
                 .WithMany(s => s.Recetas)
                 .HasForeignKey(r => r.IdSabor)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- DetallePedido ----------
            mb.Entity<DetallePedido>(e =>
            {
                e.HasOne(d => d.Pedido)
                 .WithMany(p => p.DetallePedidos)
                 .HasForeignKey(d => d.IdPedido)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(d => d.Producto)
                 .WithMany(p => p.DetallePedidos)
                 .HasForeignKey(d => d.IdProducto)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(d => d.Sabor)
                 .WithMany(s => s.DetallePedidos)
                 .HasForeignKey(d => d.IdSabor)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- DetalleCompraInsumo ----------
            mb.Entity<DetalleCompraInsumo>()
              .HasOne(d => d.CompraInsumos)
              .WithMany(c => c.Detalles)
              .HasForeignKey(d => d.CompraId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<DetalleCompraInsumo>()
              .HasOne(d => d.Insumo)
              .WithMany(i => i.DetallesCompra)
              .HasForeignKey(d => d.InsumoId)
              .OnDelete(DeleteBehavior.Restrict);

            // ---------- Nombres de tablas ----------
            mb.Entity<Dia>().ToTable("Dias");
            mb.Entity<CanalAtencion>().ToTable("CanalesAtencion");
            mb.Entity<CanalPromocion>().ToTable("CanalesPromocion");
            mb.Entity<EstadoPedido>().ToTable("EstadosPedido");
            mb.Entity<TipoPago>().ToTable("TiposPago");
            mb.Entity<TipoProducto>().ToTable("TiposProducto");
            mb.Entity<TipoPromocion>().ToTable("TiposPromocion");
            mb.Entity<ZonaLocal>().ToTable("ZonasLocal");
            mb.Entity<ZonaDelivery>().ToTable("ZonasDelivery");
            mb.Entity<NroMesa>().ToTable("Mesas");
            mb.Entity<CategoriaProducto>().ToTable("CategoriasProducto");
            mb.Entity<Sabor>().ToTable("Sabores");
            mb.Entity<Producto>().ToTable("Productos");
            mb.Entity<ComboProducto>().ToTable("ComboProductos");
            mb.Entity<Receta>().ToTable("Recetas");
            mb.Entity<Promocion>().ToTable("Promociones");
            mb.Entity<Cliente>().ToTable("Clientes");
            mb.Entity<Insumo>().ToTable("Insumos");
            mb.Entity<CompraInsumos>().ToTable("ComprasInsumos");
            mb.Entity<DetalleCompraInsumo>().ToTable("DetallesCompraInsumo");
            mb.Entity<Pedido>().ToTable("Pedidos");
            mb.Entity<DetallePedido>().ToTable("DetallesPedido");
            mb.Entity<PedidoLocal>().ToTable("PedidosLocal");
            mb.Entity<PedidoDelivery>().ToTable("PedidosDelivery");
            mb.Entity<Pago>().ToTable("Pagos");

            // ---------- Seed ----------
            mb.Entity<TipoProducto>().HasData(
                new TipoProducto { IdTipoProducto = 1, Nombre = "Alita", Activo = true },
                new TipoProducto { IdTipoProducto = 2, Nombre = "Bebida", Activo = true },
                new TipoProducto { IdTipoProducto = 3, Nombre = "Guarnicion", Activo = true },
                new TipoProducto { IdTipoProducto = 4, Nombre = "Combo", Activo = true }
            );

            mb.Entity<TipoPromocion>().HasData(
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Precio normal", Codigo = "NORMAL", Activo = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "Promocion docena", Codigo = "DOCENA", Activo = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Promocion 2x1", Codigo = "2X1", Activo = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Precio combo", Codigo = "COMBO", Activo = true }
            );

            mb.Entity<Dia>().HasData(
                new Dia { IdDia = 1, Nombre = "Lunes", Activo = true },
                new Dia { IdDia = 2, Nombre = "Martes", Activo = true },
                new Dia { IdDia = 3, Nombre = "Miercoles", Activo = true },
                new Dia { IdDia = 4, Nombre = "Jueves", Activo = true },
                new Dia { IdDia = 5, Nombre = "Viernes", Activo = true },
                new Dia { IdDia = 6, Nombre = "Sabado", Activo = true },
                new Dia { IdDia = 7, Nombre = "Domingo", Activo = true }
            );

            mb.Entity<CanalAtencion>().HasData(
                new CanalAtencion { IdCanalAtencion = 1, Nombre = "Salon", Activo = true },
                new CanalAtencion { IdCanalAtencion = 2, Nombre = "Delivery", Activo = true }
            );

            mb.Entity<CanalPromocion>().HasData(
                new CanalPromocion { IdCanalPromocion = 1, Nombre = "Salon", Activo = true },
                new CanalPromocion { IdCanalPromocion = 2, Nombre = "Delivery", Activo = true },
                new CanalPromocion { IdCanalPromocion = 3, Nombre = "Todos", Activo = true }
            );

            mb.Entity<EstadoPedido>().HasData(
                new EstadoPedido { IdEstadoPedido = 1, Nombre = "Recibido", Activo = true },
                new EstadoPedido { IdEstadoPedido = 2, Nombre = "EnPreparacion", Activo = true },
                new EstadoPedido { IdEstadoPedido = 3, Nombre = "Listo", Activo = true },
                new EstadoPedido { IdEstadoPedido = 4, Nombre = "EnReparto", Activo = true },
                new EstadoPedido { IdEstadoPedido = 5, Nombre = "Entregado", Activo = true },
                new EstadoPedido { IdEstadoPedido = 6, Nombre = "Anulado", Activo = true }
            );

            mb.Entity<TipoPago>().HasData(
                new TipoPago { IdTipoPago = 1, Nombre = "Efectivo", Activo = true },
                new TipoPago { IdTipoPago = 2, Nombre = "Yape", Activo = true },
                new TipoPago { IdTipoPago = 3, Nombre = "Tarjeta", Activo = true }
            );

            mb.Entity<Sabor>().HasData(
                new Sabor { IdSabor = 1, Nombre = "Buffalo", Activo = true },
                new Sabor { IdSabor = 2, Nombre = "BBQ", Activo = true },
                new Sabor { IdSabor = 3, Nombre = "Acevichada", Activo = true },
                new Sabor { IdSabor = 4, Nombre = "Maracuya", Activo = true },
                new Sabor { IdSabor = 5, Nombre = "Ajo Parmesano", Activo = true },
                new Sabor { IdSabor = 6, Nombre = "Honey Mustard", Activo = true },
                new Sabor { IdSabor = 7, Nombre = "Atomica", Activo = true },
                new Sabor { IdSabor = 8, Nombre = "Picante", Activo = true }
            );
        }
    }
}
