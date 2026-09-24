using Microsoft.AspNetCore.Authorization;

namespace AlitasGoWeb.Seguridad
{
    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Cajero = "Cajero";
        public const string Cocinero = "Cocinero";
        public const string Repartidor = "Repartidor";

        public static readonly string[] Todos = { Administrador, Cajero, Cocinero, Repartidor };
    }

    public static class Politicas
    {
        public const string SoloAdmin = "SoloAdmin";
        public const string TomarPedidos = "TomarPedidos";
        public const string AnularPedido = "AnularPedido";
        public const string Cocina = "Cocina";
        public const string Reparto = "Reparto";
    }

    public static class ConfiguracionSeguridad
    {
        // Cuando el usuario entra con el código de la app autenticadora, Identity agrega el claim amr = "mfa".
        public const string ClaimMetodoAutenticacion = "amr";
        public const string ValorMfa = "mfa";

        // Matriz rol × caso de uso llevada a políticas (Semana 6, tema 5).
        public static void ConfigurarPoliticas(AuthorizationOptions op)
        {
            // Igual que el material (Semana 3): administración = rol Administrador.
            op.AddPolicy(Politicas.SoloAdmin, p => p
                .RequireRole(RolesSistema.Administrador));

            // Anular exige además haber ingresado con segundo factor. El material lo escribe como
            // RequireClaim("mfa", "true"); Identity, al validar el código 2FA, emite el claim amr = "mfa".
            op.AddPolicy(Politicas.AnularPedido, p => p
                .RequireRole(RolesSistema.Administrador)
                .RequireClaim(ClaimMetodoAutenticacion, ValorMfa));

            op.AddPolicy(Politicas.TomarPedidos, p => p
                .RequireRole(RolesSistema.Administrador, RolesSistema.Cajero));

            op.AddPolicy(Politicas.Cocina, p => p
                .RequireRole(RolesSistema.Administrador, RolesSistema.Cocinero));

            op.AddPolicy(Politicas.Reparto, p => p
                .RequireRole(RolesSistema.Administrador, RolesSistema.Repartidor));
        }
    }
}
