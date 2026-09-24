namespace AlitasGoWeb.Models
{
    /// <summary>
    /// Ciclo de vida del Pedido (Semana 3, diagrama de máquina de estados):
    /// Recibido → En preparación → Listo → (En reparto | Servido en salón) → Entregado.
    /// Desde cualquier estado previo a la entrega se puede derivar a Anulado.
    /// Entregado y Anulado son estados finales.
    /// </summary>
    public static class MaquinaEstadosPedido
    {
        public static string Nombre(int estado) => estado switch
        {
            Estados.Recibido => "Recibido",
            Estados.EnPreparacion => "En preparación",
            Estados.Listo => "Listo",
            Estados.EnReparto => "En reparto",
            Estados.Entregado => "Entregado",
            Estados.Anulado => "Anulado",
            Estados.Servido => "Servido",
            _ => "Desconocido"
        };

        public static bool TransicionValida(int actual, int nuevo, bool esDelivery)
        {
            if (nuevo == Estados.Anulado) return PuedeAnular(actual);

            return (actual, nuevo) switch
            {
                (Estados.Recibido, Estados.EnPreparacion) => true,
                (Estados.EnPreparacion, Estados.Listo) => true,
                (Estados.Listo, Estados.EnReparto) => esDelivery,
                (Estados.Listo, Estados.Servido) => !esDelivery,
                (Estados.EnReparto, Estados.Entregado) => esDelivery,
                (Estados.Servido, Estados.Entregado) => !esDelivery,
                _ => false
            };
        }

        public static bool PuedeAnular(int actual) =>
            actual is Estados.Recibido or Estados.EnPreparacion or Estados.Listo
                   or Estados.EnReparto or Estados.Servido;

        // Tabla de transiciones: "Recibido / En prep. → anular() → Reponer stock".
        // Si ya está Listo o en reparto, la comida ya se preparó: no se repone.
        public static bool DebeReponerStock(int estadoAlAnular) =>
            estadoAlAnular is Estados.Recibido or Estados.EnPreparacion;

        // "Recibido → agregarItem() [pedido no enviado a cocina]"
        public static bool PuedeEditar(int actual) => actual == Estados.Recibido;

        // "En reparto / Servido → confirmarEntrega() [pago registrado] → Entregado"
        public static bool PuedeCobrar(int actual, bool esDelivery) =>
            esDelivery ? actual == Estados.EnReparto : actual == Estados.Servido;
    }
}
