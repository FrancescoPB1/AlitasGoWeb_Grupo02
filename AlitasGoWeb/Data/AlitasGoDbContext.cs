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

            // ============================================
            // SEED
            // ============================================

            // TipoProducto
            mb.Entity<TipoProducto>().HasData(
                new TipoProducto { IdTipoProducto = 1, Nombre = "Plato", Activo = true },
                new TipoProducto { IdTipoProducto = 2, Nombre = "Bebida", Activo = true },
                new TipoProducto { IdTipoProducto = 3, Nombre = "Guarnicion", Activo = true },
                new TipoProducto { IdTipoProducto = 4, Nombre = "Combo", Activo = true }
            );

            // TipoPromocion
            mb.Entity<TipoPromocion>().HasData(
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Precio normal", Codigo = "NORMAL", Activo = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "Promocion docena", Codigo = "DOCENA", Activo = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Promocion 2x1", Codigo = "2X1", Activo = true }
            );

            // Dia
            mb.Entity<Dia>().HasData(
                new Dia { IdDia = 1, Nombre = "Lunes", Activo = true },
                new Dia { IdDia = 2, Nombre = "Martes", Activo = true },
                new Dia { IdDia = 3, Nombre = "Miercoles", Activo = true },
                new Dia { IdDia = 4, Nombre = "Jueves", Activo = true },
                new Dia { IdDia = 5, Nombre = "Viernes", Activo = true },
                new Dia { IdDia = 6, Nombre = "Sabado", Activo = true },
                new Dia { IdDia = 7, Nombre = "Domingo", Activo = true }
            );

            // CanalAtencion
            mb.Entity<CanalAtencion>().HasData(
                new CanalAtencion { IdCanalAtencion = 1, Nombre = "Salon", Activo = true },
                new CanalAtencion { IdCanalAtencion = 2, Nombre = "Delivery", Activo = true }
            );

            // CanalPromocion
            mb.Entity<CanalPromocion>().HasData(
                new CanalPromocion { IdCanalPromocion = 1, Nombre = "Salon", Activo = true },
                new CanalPromocion { IdCanalPromocion = 2, Nombre = "Delivery", Activo = true },
                new CanalPromocion { IdCanalPromocion = 3, Nombre = "Todos", Activo = true }
            );

            // EstadoPedido
            mb.Entity<EstadoPedido>().HasData(
                new EstadoPedido { IdEstadoPedido = 1, Nombre = "Recibido", Activo = true },
                new EstadoPedido { IdEstadoPedido = 2, Nombre = "EnPreparacion", Activo = true },
                new EstadoPedido { IdEstadoPedido = 3, Nombre = "Listo", Activo = true },
                new EstadoPedido { IdEstadoPedido = 4, Nombre = "EnReparto", Activo = true },
                new EstadoPedido { IdEstadoPedido = 5, Nombre = "Entregado", Activo = true },
                new EstadoPedido { IdEstadoPedido = 6, Nombre = "Anulado", Activo = true }
            );

            // TipoPago
            mb.Entity<TipoPago>().HasData(
                new TipoPago { IdTipoPago = 1, Nombre = "Efectivo", Activo = true },
                new TipoPago { IdTipoPago = 2, Nombre = "Yape", Activo = true },
                new TipoPago { IdTipoPago = 3, Nombre = "Tarjeta", Activo = true }
            );

            // Sabor
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

            // CategoriaProducto
            mb.Entity<CategoriaProducto>().HasData(
                new CategoriaProducto { IdCategoriaProducto = 1, Nombre = "Alitas", Activo = true },
                new CategoriaProducto { IdCategoriaProducto = 2, Nombre = "Boneless", Activo = true },
                new CategoriaProducto { IdCategoriaProducto = 3, Nombre = "Guarniciones", Activo = true },
                new CategoriaProducto { IdCategoriaProducto = 4, Nombre = "Bebidas", Activo = true }
            );

            // Cliente
            mb.Entity<Cliente>().HasData(
                new Cliente { IdCliente = 1, DNI = "12345678", Nombre = "Juan", ApellidoPaterno = "Perez", ApellidoMaterno = "Gomez", Activo = true },
                new Cliente { IdCliente = 2, DNI = "87654321", Nombre = "Maria", ApellidoPaterno = "Lopez", ApellidoMaterno = "Diaz", Activo = true },
                new Cliente { IdCliente = 3, DNI = "11223344", Nombre = "Carlos", ApellidoPaterno = "Ramirez", ApellidoMaterno = "Torres", Activo = true }
            );

            // Producto
            mb.Entity<Producto>().HasData(
                // Alitas
                new Producto { IdProducto = 1, IdCategoriaProducto = 1, Nombre = "Media Docena Alitas", IdTipoProducto = 1, Descripcion = "Media docena de alitas", Precio = 18.00m, RequiereSabor = true, Activo = true },
                new Producto { IdProducto = 2, IdCategoriaProducto = 1, Nombre = "Docena Alitas", IdTipoProducto = 1, Descripcion = "Docena de alitas", Precio = 30.00m, RequiereSabor = true, Activo = true },
                new Producto { IdProducto = 3, IdCategoriaProducto = 1, Nombre = "2 Docenas Alitas", IdTipoProducto = 1, Descripcion = "Dos docenas de alitas", Precio = 55.00m, RequiereSabor = true, Activo = true },
                // Boneless
                new Producto { IdProducto = 4, IdCategoriaProducto = 2, Nombre = "Media Docena Boneless", IdTipoProducto = 1, Descripcion = "Media docena boneless", Precio = 20.00m, RequiereSabor = true, Activo = true },
                new Producto { IdProducto = 5, IdCategoriaProducto = 2, Nombre = "Docena Boneless", IdTipoProducto = 1, Descripcion = "Docena boneless", Precio = 35.00m, RequiereSabor = true, Activo = true },
                // Guarniciones
                new Producto { IdProducto = 6, IdCategoriaProducto = 3, Nombre = "Papas Fritas", IdTipoProducto = 3, Descripcion = "Porcion de papas", Precio = 8.00m, RequiereSabor = false, Activo = true },
                new Producto { IdProducto = 7, IdCategoriaProducto = 3, Nombre = "Yucas Fritas", IdTipoProducto = 3, Descripcion = "Porcion de yucas", Precio = 9.00m, RequiereSabor = false, Activo = true },
                new Producto { IdProducto = 8, IdCategoriaProducto = 3, Nombre = "Ensalada", IdTipoProducto = 3, Descripcion = "Ensalada fresca", Precio = 7.00m, RequiereSabor = false, Activo = true },
                // Bebidas
                new Producto { IdProducto = 9, IdCategoriaProducto = 4, Nombre = "Gaseosa Personal", IdTipoProducto = 2, Descripcion = "Gaseosa 500ml", Precio = 5.00m, RequiereSabor = false, Activo = true },
                new Producto { IdProducto = 10, IdCategoriaProducto = 4, Nombre = "Gaseosa 1.5L", IdTipoProducto = 2, Descripcion = "Gaseosa 1.5L", Precio = 12.00m, RequiereSabor = false, Activo = true },
                new Producto { IdProducto = 11, IdCategoriaProducto = 4, Nombre = "Chicha Vaso", IdTipoProducto = 2, Descripcion = "Vaso de chicha", Precio = 4.00m, RequiereSabor = false, Activo = true },
                new Producto { IdProducto = 12, IdCategoriaProducto = 4, Nombre = "Chicha Jarra 1L", IdTipoProducto = 2, Descripcion = "Jarra de chicha", Precio = 10.00m, RequiereSabor = false, Activo = true },
                // Combo
                new Producto { IdProducto = 13, IdCategoriaProducto = 1, Nombre = "Combo Duo Alitas", IdTipoProducto = 4, Descripcion = "2 docenas + 2 gaseosas personales", Precio = 65.00m, RequiereSabor = false, Activo = true }
            );

            // ComboProducto (componentes del combo)
            mb.Entity<ComboProducto>().HasData(
                new { IdProductoCombo = 13, IdProductoComponente = 2, Cantidad = 2, IdSabor = (int?)1 },
                new { IdProductoCombo = 13, IdProductoComponente = 9, Cantidad = 2, IdSabor = (int?)null }
            );

            // ZonaLocal
            mb.Entity<ZonaLocal>().HasData(
                new ZonaLocal { IdZonaLocal = 1, Nombre = "Interior", Activo = true },
                new ZonaLocal { IdZonaLocal = 2, Nombre = "Vereda", Activo = true }
            );

            // NroMesa
            mb.Entity<NroMesa>().HasData(
                new NroMesa { IdNroMesa = 1, IdZonaLocal = 1, NumeroMesa = 1, Activo = true },
                new NroMesa { IdNroMesa = 2, IdZonaLocal = 1, NumeroMesa = 2, Activo = true },
                new NroMesa { IdNroMesa = 3, IdZonaLocal = 1, NumeroMesa = 3, Activo = true },
                new NroMesa { IdNroMesa = 4, IdZonaLocal = 1, NumeroMesa = 4, Activo = true },
                new NroMesa { IdNroMesa = 5, IdZonaLocal = 1, NumeroMesa = 5, Activo = true },
                new NroMesa { IdNroMesa = 6, IdZonaLocal = 2, NumeroMesa = 6, Activo = true },
                new NroMesa { IdNroMesa = 7, IdZonaLocal = 2, NumeroMesa = 7, Activo = true },
                new NroMesa { IdNroMesa = 8, IdZonaLocal = 2, NumeroMesa = 8, Activo = true },
                new NroMesa { IdNroMesa = 9, IdZonaLocal = 2, NumeroMesa = 9, Activo = true },
                new NroMesa { IdNroMesa = 10, IdZonaLocal = 2, NumeroMesa = 10, Activo = true }
            );

            // ZonaDelivery
            mb.Entity<ZonaDelivery>().HasData(
                new ZonaDelivery { IdZonaDelivery = 1, NombreZona = "Cerca", CostoDelivery = 3.00m, Activo = true },
                new ZonaDelivery { IdZonaDelivery = 2, NombreZona = "Media", CostoDelivery = 5.00m, Activo = true },
                new ZonaDelivery { IdZonaDelivery = 3, NombreZona = "Bordes", CostoDelivery = 8.00m, Activo = true }
            );

            // Insumo (opcional, para probar recetas después)
            mb.Entity<Insumo>().HasData(
                new Insumo { IdInsumo = 1, Nombre = "Alitas crudas", UnidadMedida = "kg", StockActual = 20, StockMinimo = 5 },
                new Insumo { IdInsumo = 2, Nombre = "Salsa buffalo", UnidadMedida = "lt", StockActual = 5, StockMinimo = 2 },
                new Insumo { IdInsumo = 3, Nombre = "Salsa BBQ", UnidadMedida = "lt", StockActual = 4, StockMinimo = 2 },
                new Insumo { IdInsumo = 4, Nombre = "Gaseosa personal", UnidadMedida = "unidad", StockActual = 50, StockMinimo = 12 }
            );
        }
    }
}
