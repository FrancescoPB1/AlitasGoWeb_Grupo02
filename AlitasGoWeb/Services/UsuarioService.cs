using AlitasGoWeb.Seguridad;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Services
{
    // Cada persona con su propio acceso y solo el rol que le corresponde.
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<IdentityUser> _usuarios;
        private readonly IAuditoriaService _auditoria;

        public UsuarioService(UserManager<IdentityUser> usuarios, IAuditoriaService auditoria)
        {
            _usuarios = usuarios;
            _auditoria = auditoria;
        }

        public async Task<List<UsuarioResumen>> ListarAsync()
        {
            var lista = await _usuarios.Users.OrderBy(u => u.Email).ToListAsync();
            var resultado = new List<UsuarioResumen>();
            foreach (var u in lista)
            {
                var roles = await _usuarios.GetRolesAsync(u);
                var bloqueado = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow;
                resultado.Add(new UsuarioResumen(u.Id, u.Email ?? u.UserName ?? "", roles.FirstOrDefault() ?? "(sin rol)",
                    u.TwoFactorEnabled, bloqueado));
            }
            return resultado;
        }

        public async Task<Resultado> CrearAsync(string email, string password, string rol, string usuarioActual)
        {
            if (!RolesSistema.Todos.Contains(rol)) return Resultado.Error("Selecciona un rol válido.");

            // Creado por el Administrador: la cuenta queda confirmada.
            var usuario = new IdentityUser { UserName = email.Trim(), Email = email.Trim(), EmailConfirmed = true };
            var creado = await _usuarios.CreateAsync(usuario, password);
            if (!creado.Succeeded) return Resultado.Error(creado.Errors.Select(e => e.Description).ToArray());

            await _usuarios.AddToRoleAsync(usuario, rol);
            await _auditoria.RegistrarYGuardarAsync(usuarioActual, "Crear usuario", "Usuario", usuario.Id, $"{email} · rol {rol}");
            return Resultado.Ok($"Usuario {email} creado con rol {rol}.");
        }

        public async Task<Resultado> CambiarRolAsync(string idUsuario, string rol, string usuarioActual)
        {
            if (!RolesSistema.Todos.Contains(rol)) return Resultado.Error("Selecciona un rol válido.");

            var usuario = await _usuarios.FindByIdAsync(idUsuario);
            if (usuario == null) return Resultado.Error("El usuario no existe.");
            if (string.Equals(usuario.UserName, usuarioActual, StringComparison.OrdinalIgnoreCase))
                return Resultado.Error("No puedes cambiar tu propio rol (evita quedarte sin acceso de administrador).");

            var actuales = await _usuarios.GetRolesAsync(usuario);
            await _usuarios.RemoveFromRolesAsync(usuario, actuales);
            await _usuarios.AddToRoleAsync(usuario, rol);
            await _usuarios.UpdateSecurityStampAsync(usuario);   // invalida la sesión con el rol anterior

            await _auditoria.RegistrarYGuardarAsync(usuarioActual, "Cambiar rol", "Usuario", usuario.Id,
                $"{usuario.Email}: {string.Join(",", actuales)} → {rol}");
            return Resultado.Ok($"{usuario.Email} ahora es {rol}.");
        }
    }
}
