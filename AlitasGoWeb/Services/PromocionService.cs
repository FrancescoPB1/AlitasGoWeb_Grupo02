using AlitasGoWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Services
{
    public class PromocionService:IPromocionService
    {
        private readonly AlitasGoDbContext _ctx;

        public PromocionService(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<string?> ObtenerCodigoAplicableAsync(int idProducto, int idCanal, DateTime fecha)
        {
            // Nombre del día en español, tal como está en la tabla Dias
            var cultura = new System.Globalization.CultureInfo("es-PE");
            var nombreDia = fecha.ToString("dddd", cultura);
            nombreDia = char.ToUpper(nombreDia[0]) + nombreDia.Substring(1);

            // Nombre del canal del pedido ("Salon" o "Delivery")
            var nombreCanal = await _ctx.CanalesAtencion
                .Where(c => c.IdCanalAtencion == idCanal)
                .Select(c => c.Nombre)
                .FirstOrDefaultAsync();

            if (nombreCanal == null) return null;

            // Buscar promo activa que coordine producto + canal + día
            var codigo = await _ctx.Promociones
                .Include(p => p.TipoPromocion)
                .Include(p => p.Dia)
                .Include(p => p.CanalPromocion)
                .Include(p => p.Productos)
                .Where(p => p.Activo
                         && p.Dia!.Nombre == nombreDia
                         && (p.CanalPromocion!.Nombre == "Todos"
                             || p.CanalPromocion.Nombre == nombreCanal)
                         && p.Productos.Any(pr => pr.IdProducto == idProducto))
                .Select(p => p.TipoPromocion!.Codigo)
                .FirstOrDefaultAsync();

            return codigo;
        }
    }
}
