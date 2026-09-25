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

        // Resumen
        public int Cantidad => Pagos.Count;
        public decimal TotalCobrado => Pagos.Sum(p => p.Monto);
        public IEnumerable<(string Medio, int Cantidad, decimal Total)> PorMedio =>
            Pagos.GroupBy(p => p.TipoPago?.Nombre ?? "—")
                 .Select(g => (g.Key, g.Count(), g.Sum(p => p.Monto)))
                 .OrderByDescending(x => x.Item3);
    }
}
