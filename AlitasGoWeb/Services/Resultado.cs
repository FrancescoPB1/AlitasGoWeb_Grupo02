namespace AlitasGoWeb.Services
{
    // Resultado de un caso de uso: el controlador lo traduce a ModelState o TempData.
    public class Resultado
    {
        public List<string> Errores { get; } = new();
        public string? Mensaje { get; set; }
        public bool Exito => Errores.Count == 0;

        public static Resultado Ok(string? mensaje = null) => new() { Mensaje = mensaje };

        public static Resultado Error(params string[] errores)
        {
            var r = new Resultado();
            r.Errores.AddRange(errores);
            return r;
        }
    }

    public class Resultado<T> : Resultado
    {
        public T? Valor { get; set; }

        public static Resultado<T> Ok(T valor, string? mensaje = null) => new() { Valor = valor, Mensaje = mensaje };

        public static new Resultado<T> Error(params string[] errores)
        {
            var r = new Resultado<T>();
            r.Errores.AddRange(errores);
            return r;
        }
    }
}
