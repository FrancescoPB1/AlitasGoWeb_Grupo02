namespace AlitasGoWeb.Services
{
    // Singleton (Semana 4, bloque 7): datos del local que casi nunca cambian
    // y que aparecen en todos los comprobantes. Se configuran en un solo lugar.
    public sealed class ConfiguracionSistema : IConfiguracionSistema
    {
        private static readonly Lazy<ConfiguracionSistema> _instancia =
            new(() => new ConfiguracionSistema());

        public static ConfiguracionSistema Instancia => _instancia.Value;

        public decimal Igv { get; } = 0.18m;
        public string NombreNegocio { get; } = "AlitasGo";
        public string Ruc { get; } = "20512345678";
        public string Direccion { get; } = "Trujillo, La Libertad";
        public string Moneda { get; } = "PEN";
        public string SimboloMoneda { get; } = "S/";
        public string SerieBoleta { get; } = "B001";
        public string SerieFactura { get; } = "F001";

        private ConfiguracionSistema() { }
    }
}
