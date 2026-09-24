namespace AlitasGoWeb.Services
{
    public interface IConfiguracionSistema
    {
        decimal Igv { get; }
        string NombreNegocio { get; }
        string Ruc { get; }
        string Direccion { get; }
        string Moneda { get; }
        string SimboloMoneda { get; }
        string SerieBoleta { get; }
        string SerieFactura { get; }
    }
}
