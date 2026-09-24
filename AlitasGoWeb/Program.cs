using System.Globalization;
using AlitasGoWeb.Data;
using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Hubs;
using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.Services.Precios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cultura = new CultureInfo("es-PE");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

builder.Services.AddDbContext<AlitasGoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AlitasGoConnection") ??
        throw new InvalidOperationException("Connection string 'AlitasGoConnection' not found."))
);

// 1) Identity: reglas de contraseña, bloqueo tras intentos fallidos y roles del negocio
builder.Services.AddDefaultIdentity<IdentityUser>(o =>
{
    o.Password.RequiredLength = 10;
    o.Password.RequireDigit = true;
    o.Password.RequireUppercase = true;
    o.Password.RequireNonAlphanumeric = true;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    o.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddErrorDescriber<ErroresIdentityEspanol>()
.AddEntityFrameworkStores<AlitasGoDbContext>();

// 2) Cookie de sesión endurecida
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.HttpOnly = true;                              // no visible a JavaScript
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;     // solo por HTTPS
    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    o.SlidingExpiration = true;
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/Cuenta/AccesoDenegado";         // registra el intento en la bitácora
});

// 3) Políticas: reglas de negocio, no solo roles
builder.Services.AddAuthorization(ConfiguracionSeguridad.ConfigurarPoliticas);

// Caché del catálogo (driver de rendimiento en hora punta)
builder.Services.AddMemoryCache();

// Repositories + Unit of Work
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

// Strategies
builder.Services.AddScoped<IEstrategiaPrecio, PrecioNormal>();
builder.Services.AddScoped<IEstrategiaPrecio, PromocionDocena>();
builder.Services.AddScoped<IEstrategiaPrecio, Promocion2x1>();
builder.Services.AddScoped<IEstrategiaPrecio, PrecioCombo>();

// Services
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IPromocionService, PromocionService>();
builder.Services.AddScoped<IPrecioService, PrecioService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICalculadoraIgv, CalculadoraIgv>();

// Tiempo real (SignalR, incluido en ASP.NET Core): cocina, salón y reparto se actualizan solos
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificadorPedidos, NotificadorPedidosSignalR>();

// Singleton
builder.Services.AddSingleton<IConfiguracionSistema>(ConfiguracionSistema.Instancia);
builder.Services.AddSingleton<IReloj, RelojSistema>();

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var m = options.ModelBindingMessageProvider;
    m.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    m.SetMissingBindRequiredValueAccessor(_ => "Este campo es obligatorio.");
    m.SetAttemptedValueIsInvalidAccessor((v, _) => $"El valor '{v}' no es válido.");
    m.SetValueIsInvalidAccessor(v => $"El valor '{v}' no es válido.");
    m.SetUnknownValueIsInvalidAccessor(_ => "El valor ingresado no es válido.");
    m.SetNonPropertyValueMustBeANumberAccessor(() => "Debe ser un número.");
    m.SetValueMustBeANumberAccessor(_ => "Debe ser un número.");
});
builder.Services.AddRazorPages();   // páginas de Identity (login, 2FA)

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inicio/Error");   // página de error genérica: no expone detalles
    app.UseHsts();
}

// 4) EL ORDEN DEL PIPELINE IMPORTA
app.UseHttpsRedirection();   // Secure Pipe
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();     // ¿quién eres?
app.UseAuthorization();      // ¿puedes hacerlo?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<PedidosHub>(PedidosHub.Ruta);

try
{
    await SemillaIdentidad.InicializarAsync(app.Services, app.Configuration, app.Logger);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "No se pudieron sembrar roles y usuarios. ¿Ejecutó Update-Database?");
}

app.Run();
