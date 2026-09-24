namespace AlitasGoWeb.Models
{
    // Identificadores sembrados en EstadosPedido (ver AlitasGoDbContext). Evita "números mágicos".
    public static class Estados
    {
        public const int Recibido = 1;
        public const int EnPreparacion = 2;
        public const int Listo = 3;
        public const int EnReparto = 4;
        public const int Entregado = 5;
        public const int Anulado = 6;
        public const int Servido = 7;   // Servido en salón: la mesa comió pero aún no paga
    }

    // Identificadores sembrados en CanalesAtencion.
    public static class Canales
    {
        public const int Salon = 1;
        public const int Delivery = 2;
    }

    public static class TiposComprobante
    {
        public const string Boleta = "Boleta";
        public const string Factura = "Factura";
    }

    public static class TiposMovimiento
    {
        public const string Entrada = "Entrada";
        public const string Salida = "Salida";
    }
}
