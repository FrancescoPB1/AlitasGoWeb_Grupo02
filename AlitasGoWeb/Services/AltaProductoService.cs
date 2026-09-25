using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // Crea el producto y su receta en UNA sola transacción: se guarda todo o no se guarda nada.
    // Reutiliza las reglas que ya existen: CatalogoService valida el producto e
    // InventarioService valida y registra cada línea de la receta (con su bitácora).
    public class AltaProductoService : IAltaProductoService
    {
        private readonly ICatalogoService _catalogo;
        private readonly IInventarioService _inventario;
        private readonly IUnidadDeTrabajo _uow;

        public AltaProductoService(ICatalogoService catalogo, IInventarioService inventario, IUnidadDeTrabajo uow)
        {
            _catalogo = catalogo;
            _inventario = inventario;
            _uow = uow;
        }

        public async Task<Resultado> CrearConRecetaAsync(Producto producto, IList<LineaIngrediente> ingredientes, string usuario)
        {
            // Las filas que quedaron vacías en el formulario no cuentan.
            var lineas = ingredientes.Where(l => l.IdInsumo != 0 || l.Cantidad != 0).ToList();

            // Un combo no tiene receta propia: consume la de los productos que lo forman.
            if (producto.EsCombo) lineas.Clear();
            else
            {
                var errores = await ValidarAsync(producto, lineas);
                if (errores.Count > 0) return Resultado.Error(errores.ToArray());
            }

            try
            {
                await _uow.EjecutarEnTransaccionAsync(async () =>
                {
                    var creado = await _catalogo.CrearProductoAsync(producto, usuario);
                    if (!creado.Exito) throw new AltaCancelada(creado);

                    foreach (var l in lineas)
                    {
                        var r = await _inventario.AgregarLineaRecetaAsync(producto.IdProducto, l.IdInsumo, l.IdSabor, l.Cantidad, usuario);
                        if (!r.Exito) throw new AltaCancelada(r);
                    }
                });
            }
            catch (AltaCancelada ex)
            {
                return ex.Resultado;   // la transacción ya se deshizo: no quedó nada a medias
            }

            return Resultado.Ok(lineas.Count == 0
                ? $"Producto «{producto.Nombre}» creado."
                : $"Producto «{producto.Nombre}» creado con {lineas.Count} ingrediente(s) en su receta.");
        }

        private async Task<List<string>> ValidarAsync(Producto producto, List<LineaIngrediente> lineas)
        {
            var errores = new List<string>();
            if (lineas.Count == 0)
            {
                errores.Add("Agrega al menos un ingrediente: es lo que se descuenta del stock al vender el producto.");
                return errores;
            }

            var insumos = (await _inventario.ListarInsumosAsync()).ToDictionary(i => i.IdInsumo);
            var sabores = (await _catalogo.ListarSaboresAsync()).Select(s => s.IdSabor).ToHashSet();

            for (var i = 0; i < lineas.Count; i++)
            {
                var l = lineas[i];
                var n = i + 1;
                if (!insumos.ContainsKey(l.IdInsumo))
                    errores.Add($"Ingrediente {n}: selecciona un insumo.");
                if (l.Cantidad <= 0)
                    errores.Add($"Ingrediente {n}: la cantidad debe ser mayor que cero.");
                if (l.IdSabor.HasValue && !producto.RequiereSabor)
                    errores.Add($"Ingrediente {n}: el producto no lleva sabor; deja «Todos los sabores».");
                else if (l.IdSabor.HasValue && !sabores.Contains(l.IdSabor.Value))
                    errores.Add($"Ingrediente {n}: selecciona un sabor válido.");
            }

            foreach (var repetido in lineas.GroupBy(l => (l.IdInsumo, l.IdSabor)).Where(g => g.Count() > 1))
            {
                var nombre = insumos.TryGetValue(repetido.Key.IdInsumo, out var ins) ? ins.Nombre : "un insumo";
                errores.Add($"«{nombre}» está repetido para el mismo sabor: súmalo en una sola línea.");
            }

            return errores;
        }

        // Sirve para deshacer la transacción cuando una regla de negocio falla a mitad del alta.
        private sealed class AltaCancelada : Exception
        {
            public Resultado Resultado { get; }
            public AltaCancelada(Resultado resultado) : base(string.Join(" ", resultado.Errores)) { Resultado = resultado; }
        }
    }
}
