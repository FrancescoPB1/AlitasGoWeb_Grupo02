namespace AlitasGoWeb.Services.Dtos
{
    // Un producto dentro de un combo: cuántas unidades lleva y, si el producto lo pide, de qué sabor.
    public class LineaComponente
    {
        public int IdProducto { get; set; }
        public int? IdSabor { get; set; }
        public int Cantidad { get; set; }
    }
}
