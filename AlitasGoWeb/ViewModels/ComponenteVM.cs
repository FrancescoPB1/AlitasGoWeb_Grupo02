using AlitasGoWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlitasGoWeb.ViewModels
{
    // Una fila de la sección «Productos del combo» del formulario de nuevo producto.
    public class ComponenteVM
    {
        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [Display(Name = "Sabor")]
        public int? IdSabor { get; set; }

        [Display(Name = "Cantidad")]
        [Range(0, 99, ErrorMessage = "Cantidad no válida.")]
        public int Cantidad { get; set; } = 1;
    }

    // Pantalla «Productos del combo»: lo que ya tiene y el formulario para agregar uno más.
    public class ComponentesComboVM
    {
        public Producto Combo { get; set; } = null!;
        public List<ComboProducto> Lineas { get; set; } = new();
        public ComponenteVM Nuevo { get; set; } = new();
        public List<Producto> Productos { get; set; } = new();       // los que se pueden poner en un combo
        public IEnumerable<SelectListItem> Sabores { get; set; } = new List<SelectListItem>();
    }
}
