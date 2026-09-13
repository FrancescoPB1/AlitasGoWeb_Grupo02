namespace AlitasGoWeb.Services
{
    public sealed class ConfiguracionSistema:IConfiguracionSistema
    {
        private static readonly Lazy<ConfiguracionSistema> _instancia =
    new(() => new ConfiguracionSistema());

        public static ConfiguracionSistema Instancia => _instancia.Value;

        public decimal Igv { get; } = 0.18m;
        public string NombreNegocio { get; } = "AlitasGo";
        public string Ruc { get; } = "20512345678";

        private ConfiguracionSistema() { }
    }
}
