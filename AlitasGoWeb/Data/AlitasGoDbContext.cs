using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data
{
    public class AlitasGoDbContext:DbContext
    {
        public AlitasGoDbContext(DbContextOptions<AlitasGoDbContext> options) : base(options)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<CategoriaProducto> CategoriaProductos { get; set; }
        public DbSet<TipoProducto> TipoProductos { get; set; }
        public DbSet<ProductoInsumo> ProductoInsumos { get; set; }
        public DbSet<PresentacionProducto> PresentacionProductos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoProducto>().HasData(
                new TipoProducto { IdTipoProducto = 1, Nombre = "Simple", Activo = true },
                new TipoProducto { IdTipoProducto = 2, Nombre = "Combo", Activo = true }
            );
        }
    }
}
