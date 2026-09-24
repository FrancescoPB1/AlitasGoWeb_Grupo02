using Microsoft.AspNetCore.Identity;

namespace AlitasGoWeb.Seguridad
{
    // Siembra de roles y del primer administrador desde código (no a mano en la base de datos).
    // La contraseña inicial NO va en Git: se lee de User Secrets o de una variable de entorno.
    public static class SemillaIdentidad
    {
        public const string ClavePassword = "Seed:PasswordInicial";

        public static readonly (string Email, string Rol)[] UsuariosIniciales =
        {
            ("rosa@alitasgo.pe", RolesSistema.Administrador),
            ("kevin@alitasgo.pe", RolesSistema.Cajero),
            ("italo@alitasgo.pe", RolesSistema.Cocinero),
            ("brayan@alitasgo.pe", RolesSistema.Repartidor)
        };

        public static async Task InicializarAsync(IServiceProvider servicios, IConfiguration configuracion, ILogger logger)
        {
            using var scope = servicios.CreateScope();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var usuarios = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var rol in RolesSistema.Todos)
            {
                if (!await roles.RoleExistsAsync(rol))
                    await roles.CreateAsync(new IdentityRole(rol));
            }

            var password = configuracion[ClavePassword];
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("No se crearon usuarios iniciales: configure '{Clave}' con dotnet user-secrets.", ClavePassword);
                return;
            }

            foreach (var (email, rol) in UsuariosIniciales)
            {
                if (await usuarios.FindByEmailAsync(email) != null) continue;

                var usuario = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var creado = await usuarios.CreateAsync(usuario, password);
                if (!creado.Succeeded)
                {
                    logger.LogError("No se pudo crear {Email}: {Errores}", email,
                        string.Join("; ", creado.Errors.Select(e => e.Description)));
                    continue;
                }
                await usuarios.AddToRoleAsync(usuario, rol);
            }
        }
    }
}
