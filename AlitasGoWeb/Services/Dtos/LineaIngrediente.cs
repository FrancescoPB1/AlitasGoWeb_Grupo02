namespace AlitasGoWeb.Services.Dtos
{
    // Una línea de la receta que se arma al crear un producto:
    // cuánto de un insumo se descuenta por cada unidad vendida (opcionalmente, solo para un sabor).
    public class LineaIngrediente
    {
        public int IdInsumo { get; set; }
        public int? IdSabor { get; set; }
        public decimal Cantidad { get; set; }
    }
}
