using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlitasGoWeb.ViewModels
{
    public class PedidoCreateVM
    {
        [Required(ErrorMessage = "Selecciona un cliente.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Selecciona un canal.")]
        [Display(Name = "Canal de atención")]
        public int IdCanalAtencion { get; set; }

        // Delivery
        [Display(Name = "Zona de delivery")]
        public int? IdZonaDelivery { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        // Local
        [Display(Name = "Mesa")]
        public int? IdNroMesa { get; set; }

        public List<DetallePedidoVM> Detalles { get; set; } = new();
        public Dictionary<int, bool> ProductoRequiereSabor { get; set; } = new();

        // Listas para dropdowns (no se validan)
        public IEnumerable<SelectListItem> Clientes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Canales { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Zonas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Mesas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Productos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sabores { get; set; } = new List<SelectListItem>();
    }

    public class DetallePedidoVM
    {
        public int IdProducto { get; set; }
        public int? IdSabor { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
