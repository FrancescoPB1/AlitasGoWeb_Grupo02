namespace AlitasGoWeb.Services
{
    // Inversión de dependencias: las promociones dependen del día; en pruebas se usa un reloj fijo.
    public interface IReloj
    {
        DateTime Ahora { get; }
    }

    public class RelojSistema : IReloj
    {
        public DateTime Ahora => DateTime.Now;
    }
}
