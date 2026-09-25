using System.ComponentModel.DataAnnotations;

namespace AlitasGoWeb.ViewModels
{
    // Una fila de la sección «Ingredientes» del formulario de nuevo producto.
    public class IngredienteVM
    {
        public int IdInsumo { get; set; }
        public int? IdSabor { get; set; }

        [Range(0, 9999, ErrorMessage = "Cantidad no válida.")]
        public decimal Cantidad { get; set; }
    }
}
