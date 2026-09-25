using System.Reflection;
using System.Security.Claims;
using AlitasGoWeb.Controllers;
using AlitasGoWeb.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AlitasGoWeb.Tests.Seguridad
{
    // "¿Cómo probarían que un rol no puede ejecutar una acción que no le corresponde?" (Semana 6)
    public class SeguridadTests
    {
        private static readonly IAuthorizationService Autorizacion = CrearServicio();

        private static IAuthorizationService CrearServicio()
        {
            var servicios = new ServiceCollection();
            servicios.AddLogging();
            servicios.AddAuthorization(ConfiguracionSeguridad.ConfigurarPoliticas);
            return servicios.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        }

        private static ClaimsPrincipal Usuario(string rol, bool conMfa = false)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, "prueba@alitasgo.pe"), new(ClaimTypes.Role, rol) };
            if (conMfa) claims.Add(new Claim(ConfiguracionSeguridad.ClaimMetodoAutenticacion, ConfiguracionSeguridad.ValorMfa));
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Identity.Application"));
        }

        private static async Task<bool> PuedeAsync(ClaimsPrincipal usuario, string politica) =>
            (await Autorizacion.AuthorizeAsync(usuario, politica)).Succeeded;

        [Theory]
        [InlineData(RolesSistema.Cocinero, Politicas.SoloAdmin, false)]
        [InlineData(RolesSistema.Cajero, Politicas.SoloAdmin, false)]
        [InlineData(RolesSistema.Cajero, Politicas.TomarPedidos, true)]
        [InlineData(RolesSistema.Cajero, Politicas.AnularPedido, false)]
        [InlineData(RolesSistema.Cajero, Politicas.Cocina, false)]
        [InlineData(RolesSistema.Cocinero, Politicas.Cocina, true)]
        [InlineData(RolesSistema.Cocinero, Politicas.TomarPedidos, false)]
        [InlineData(RolesSistema.Repartidor, Politicas.Reparto, true)]
        [InlineData(RolesSistema.Repartidor, Politicas.TomarPedidos, false)]
        [InlineData(RolesSistema.Administrador, Politicas.TomarPedidos, true)]
        [InlineData(RolesSistema.Administrador, Politicas.SoloAdmin, true)]
        public async Task Politicas_RespetanLaMatrizRolCasoDeUso(string rol, string politica, bool esperado)
        {
            Assert.Equal(esperado, await PuedeAsync(Usuario(rol), politica));
        }

        // Igual que el material: SoloAdmin = rol Administrador; AnularPedido = Administrador + segundo factor.
        [Fact]
        public async Task Administrador_SinSegundoFactor_EntraAAdministracionPeroNoAnula()
        {
            Assert.True(await PuedeAsync(Usuario(RolesSistema.Administrador), Politicas.SoloAdmin));
            Assert.False(await PuedeAsync(Usuario(RolesSistema.Administrador), Politicas.AnularPedido));
        }

        [Fact]
        public async Task Administrador_ConSegundoFactor_EntraAReportesYAnula()
        {
            Assert.True(await PuedeAsync(Usuario(RolesSistema.Administrador, conMfa: true), Politicas.SoloAdmin));
            Assert.True(await PuedeAsync(Usuario(RolesSistema.Administrador, conMfa: true), Politicas.AnularPedido));
        }

        [Fact]
        public void HubDeTiempoReal_ExigeSesion()
        {
            Assert.NotNull(typeof(AlitasGoWeb.Hubs.PedidosHub).GetCustomAttribute<AuthorizeAttribute>());
        }

        [Fact]
        public async Task Cocinero_ConMfa_TampocoEntraAReportes()
        {
            Assert.False(await PuedeAsync(Usuario(RolesSistema.Cocinero, conMfa: true), Politicas.SoloAdmin));
        }

        // ---------- Los controladores están protegidos con la política correcta ----------
        private static string? PoliticaDe(MemberInfo miembro) =>
            miembro.GetCustomAttribute<AuthorizeAttribute>()?.Policy;

        [Theory]
        [InlineData(typeof(ReportesController), Politicas.SoloAdmin)]
        [InlineData(typeof(ProductoController), Politicas.SoloAdmin)]
        [InlineData(typeof(CategoriaProductoController), Politicas.SoloAdmin)]
        [InlineData(typeof(PromocionesController), Politicas.SoloAdmin)]
        [InlineData(typeof(InsumosController), Politicas.SoloAdmin)]
        [InlineData(typeof(UsuariosController), Politicas.SoloAdmin)]
        [InlineData(typeof(AuditoriaController), Politicas.SoloAdmin)]
        [InlineData(typeof(PagosController), Politicas.SoloAdmin)]
        [InlineData(typeof(PedidosController), Politicas.TomarPedidos)]
        [InlineData(typeof(SalonController), Politicas.TomarPedidos)]
        [InlineData(typeof(ClientesController), Politicas.TomarPedidos)]
        [InlineData(typeof(CocinaController), Politicas.Cocina)]
        [InlineData(typeof(RepartoController), Politicas.Reparto)]
        public void Controlador_ExigeLaPoliticaDeSuRol(Type controlador, string politica)
        {
            Assert.Equal(politica, PoliticaDe(controlador));
        }

        [Fact]
        public void AnularPedido_ExigeAdministradorConSegundoFactor()
        {
            var anular = typeof(PedidosController).GetMethod(nameof(PedidosController.Anular))!;
            Assert.Equal(Politicas.AnularPedido, PoliticaDe(anular));
        }

        [Fact]
        public void TodoControlador_ExigeSesion_SalvoLaPaginaDeError()
        {
            var controladores = typeof(ControladorBase).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Controller)) && !t.IsAbstract);

            Assert.All(controladores, c => Assert.NotNull(c.GetCustomAttribute<AuthorizeAttribute>()));

            var anonimas = controladores.SelectMany(c => c.GetMethods())
                .Where(m => m.GetCustomAttribute<AllowAnonymousAttribute>() != null)
                .Select(m => $"{m.DeclaringType!.Name}.{m.Name}")
                .ToList();
            Assert.Equal(new[] { "InicioController.Error" }, anonimas);
        }

        [Fact]
        public void TodaAccionPost_ValidaElTokenAntifalsificacion()
        {
            var postsSinToken = typeof(ControladorBase).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Controller)))
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<HttpPostAttribute>() != null
                         && m.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() == null)
                .Select(m => $"{m.DeclaringType!.Name}.{m.Name}")
                .ToList();

            Assert.Empty(postsSinToken);
        }
    }
}
