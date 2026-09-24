using AlitasGoWeb.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AlitasGoWeb.Hubs
{
    public static class GruposTiempoReal
    {
        public const string Admin = "admin";
        public const string Caja = "caja";
        public const string Cocina = "cocina";
        public const string Reparto = "reparto";
    }

    // Canal en tiempo real: el servidor empuja los cambios de los pedidos a las pantallas abiertas.
    // Solo usuarios con sesión; cada conexión se une al grupo de su rol.
    [Authorize]
    public class PedidosHub : Hub
    {
        public const string Ruta = "/hubs/pedidos";
        public const string Metodo = "pedidoActualizado";

        public override async Task OnConnectedAsync()
        {
            var usuario = Context.User;
            if (usuario != null)
            {
                if (usuario.IsInRole(RolesSistema.Administrador)) await Groups.AddToGroupAsync(Context.ConnectionId, GruposTiempoReal.Admin);
                if (usuario.IsInRole(RolesSistema.Cajero)) await Groups.AddToGroupAsync(Context.ConnectionId, GruposTiempoReal.Caja);
                if (usuario.IsInRole(RolesSistema.Cocinero)) await Groups.AddToGroupAsync(Context.ConnectionId, GruposTiempoReal.Cocina);
                if (usuario.IsInRole(RolesSistema.Repartidor)) await Groups.AddToGroupAsync(Context.ConnectionId, GruposTiempoReal.Reparto);
            }
            await base.OnConnectedAsync();
        }
    }
}
