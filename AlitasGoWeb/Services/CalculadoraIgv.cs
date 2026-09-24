namespace AlitasGoWeb.Services
{
    public record DesgloseIgv(decimal BaseImponible, decimal Igv, decimal Total);

    public interface ICalculadoraIgv
    {
        DesgloseIgv Desglosar(decimal totalConIgv);
    }

    // Fabricación pura: los precios de la carta YA incluyen IGV, así que se desglosa hacia atrás.
    public class CalculadoraIgv : ICalculadoraIgv
    {
        private readonly IConfiguracionSistema _config;

        public CalculadoraIgv(IConfiguracionSistema config)
        {
            _config = config;
        }

        public DesgloseIgv Desglosar(decimal totalConIgv)
        {
            var baseImponible = Math.Round(totalConIgv / (1 + _config.Igv), 2, MidpointRounding.AwayFromZero);
            return new DesgloseIgv(baseImponible, totalConIgv - baseImponible, totalConIgv);
        }
    }
}
