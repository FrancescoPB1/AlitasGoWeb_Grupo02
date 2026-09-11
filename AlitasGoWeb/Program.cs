using AlitasGoWeb.Data;
using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);


var cultura = new CultureInfo("es-PE");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;


builder.Services.AddDbContext<AlitasGoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AlitasGoConnection") ??
        throw new InvalidOperationException("Connection string 'AlitasGoConnection' not found."))
);


//builder.Services.AddScoped<ICategoriaProductoRepository, CategoriaProductoRepository>();
//builder.Services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();

////builder.Services.AddScoped<ITipoProductoRepository, TipoProductoRepository>();
//builder.Services.AddScoped<ITipoProductoService, TipoProductoService>();

//builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
//builder.Services.AddScoped<IProductoService, ProductoService>();

//builder.Services.AddScoped<IPresentacionProductoRepository, PresentacionProductoRepository>();
//builder.Services.AddScoped<IPresentacionProductoService, PresentacionProductoService>();

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Producto/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Producto}/{action=Index}/{id?}");

app.Run();
