using AlitasGoWeb.Models;
using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlitasGoWeb.ViewModels
{
    public class PedidosIndexVM
    {
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int? IdEstado { get; set; }
        public List<Pedido> Pedidos { get; set; } = new();
        public IEnumerable<SelectListItem> Estados { get; set; } = new List<SelectListItem>();
    }

    public class CobroVM
    {
        public int IdPedido { get; set; }
        public Pedido? Pedido { get; set; }
        public string Controlador { get; set; } = "Pedidos";
        public string Accion { get; set; } = "Cobrar";

        [Required(ErrorMessage = "Selecciona el medio de pago.")]
        [Display(Name = "Medio de pago")]
        public int? IdTipoPago { get; set; }

        [Required(ErrorMessage = "Selecciona el comprobante.")]
        [Display(Name = "Comprobante")]
        public string TipoComprobante { get; set; } = TiposComprobante.Boleta;

        [Display(Name = "RUC")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El RUC debe tener 11 dígitos.")]
        public string? Ruc { get; set; }

        [Display(Name = "Razón social")]
        [StringLength(150, ErrorMessage = "Máximo {1} caracteres.")]
        public string? RazonSocial { get; set; }

        public IEnumerable<SelectListItem> TiposPago { get; set; } = new List<SelectListItem>();
    }

    public class ComprobanteVM
    {
        public Pedido Pedido { get; set; } = null!;
        public DesgloseIgv Desglose { get; set; } = null!;
        public IConfiguracionSistema Negocio { get; set; } = null!;
    }

    public class MesaSalonVM
    {
        public NroMesa Mesa { get; set; } = null!;
        public List<Pedido> PedidosActivos { get; set; } = new();
        public bool Libre => PedidosActivos.Count == 0;
    }

    public class CompraVM
    {
        public List<LineaCompraVM> Lineas { get; set; } = new();
        public IEnumerable<SelectListItem> Insumos { get; set; } = new List<SelectListItem>();
    }

    public class LineaCompraVM
    {
        public int IdInsumo { get; set; }

        [Range(0, 99999, ErrorMessage = "Cantidad no válida.")]
        public decimal Cantidad { get; set; }

        [Range(0, 99999, ErrorMessage = "Costo no válido.")]
        public decimal CostoUnitario { get; set; }
    }

    public class RecetaVM
    {
        public Producto Producto { get; set; } = null!;
        public List<Receta> Lineas { get; set; } = new();

        [Display(Name = "Insumo")]
        public int IdInsumo { get; set; }

        [Display(Name = "Solo para el sabor")]
        public int? IdSabor { get; set; }

        [Display(Name = "Cantidad usada")]
        public decimal Cantidad { get; set; }

        public IEnumerable<SelectListItem> Insumos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sabores { get; set; } = new List<SelectListItem>();
    }

    public class PromocionFormVM
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Tipo de promoción")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el tipo.")]
        public int IdTipoPromocion { get; set; }

        [Display(Name = "Canal")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el canal.")]
        public int IdCanalPromocion { get; set; }

        [Display(Name = "Día")]
        [Range(1, 7, ErrorMessage = "Selecciona el día.")]
        public int IdDia { get; set; }

        [Display(Name = "Productos")]
        public List<int> IdsProducto { get; set; } = new();

        public IEnumerable<SelectListItem> Tipos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CanalesPromo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Dias { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Productos { get; set; } = new List<SelectListItem>();
    }

    public class UsuarioCreateVM
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un rol.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;
    }

    public class RangoFechasVM<T>
    {
        [DataType(DataType.Date)] public DateTime Desde { get; set; }
        [DataType(DataType.Date)] public DateTime Hasta { get; set; }
        public T Datos { get; set; } = default!;
    }
}
