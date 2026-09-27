using AlitasGoWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlitasGoWeb.ViewModels
{
    public class HistorialPagosVM
    {
        // Filtros
        [DataType(DataType.Date)] public DateTime Desde { get; set; }
        [DataType(DataType.Date)] public DateTime Hasta { get; set; }
        public int? IdTipoPago { get; set; }
        public string? TipoComprobante { get; set; }

        // Resultado
        public List<Pago> Pagos { get; set; } = new();
        public IEnumerable<SelectListItem> TiposPago { get; set; } = new List<SelectListItem>();

        // Resumen: un pago cuyo pedido se anuló después de cobrarse no cuenta como ingreso.
        public static bool EstaAnulado(Pago p) => p.Pedido?.IdEstadoPedido == Estados.Anulado;
        public IEnumerable<Pago> Vigentes => Pagos.Where(p => !EstaAnulado(p));
        public int Cantidad => Vigentes.Count();
        public int Anulados => Pagos.Count(EstaAnulado);
        public decimal TotalCobrado => Vigentes.Sum(p => p.Monto);
        public IEnumerable<(string Medio, int Cantidad, decimal Total)> PorMedio =>
            Vigentes.GroupBy(p => p.TipoPago?.Nombre ?? "—")
                 .Select(g => (g.Key, g.Count(), g.Sum(p => p.Monto)))
                 .OrderByDescending(x => x.Item3);
    }
}
