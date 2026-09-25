using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    public class ComboService : IComboService
    {
        public const int CantidadMaxima = 20;

        private readonly IComboRepository _repo;
        private readonly ICatalogoService _catalogo;
        private readonly IAuditoriaService _auditoria;
        private readonly IUnidadDeTrabajo _uow;

        public ComboService(IComboRepository repo, ICatalogoService catalogo, IAuditoriaService auditoria, IUnidadDeTrabajo uow)
        {
            _repo = repo;
            _catalogo = catalogo;
            _auditoria = auditoria;
            _uow = uow;
        }

        public Task<List<ComboProducto>> ListarComponentesAsync(int idCombo) => _repo.ListarComponentesAsync(idCombo);

        // Reglas de un combo. idCombo = 0 cuando el combo aún no existe (se está creando).
        public async Task<List<string>> ValidarComponentesAsync(int idCombo, IList<LineaComponente> lineas)
        {
            var errores = new List<string>();
            if (lineas.Count == 0)
            {
                errores.Add("Agrega al menos un producto al combo: de ellos sale el stock que descuenta.");
                return errores;
            }

            var productos = (await _catalogo.ListarProductosAsync(false)).ToDictionary(p => p.IdProducto);
            var sabores = (await _catalogo.ListarSaboresAsync()).Select(s => s.IdSabor).ToHashSet();

            for (var i = 0; i < lineas.Count; i++)
            {
                var l = lineas[i];
                var n = $"Producto {i + 1}";
                if (!productos.TryGetValue(l.IdProducto, out var p) || !p.Activo)
                {
                    errores.Add($"{n}: selecciona un producto activo.");
                    continue;
                }
                if (p.EsCombo || p.IdProducto == idCombo)
                    errores.Add($"{n}: «{p.Nombre}» es un combo; un combo no puede contener otro combo.");
                if (l.Cantidad < 1 || l.Cantidad > CantidadMaxima)
                    errores.Add($"{n}: la cantidad debe estar entre 1 y {CantidadMaxima}.");
                if (p.RequiereSabor && !l.IdSabor.HasValue)
                    errores.Add($"{n}: «{p.Nombre}» lleva sabor; elige cuál va en el combo.");
                else if (!p.RequiereSabor && l.IdSabor.HasValue)
                    errores.Add($"{n}: «{p.Nombre}» no lleva sabor; deja «Sin sabor».");
                else if (l.IdSabor.HasValue && !sabores.Contains(l.IdSabor.Value))
                    errores.Add($"{n}: selecciona un sabor válido.");
            }

            foreach (var repetido in lineas.GroupBy(l => l.IdProducto).Where(g => g.Count() > 1))
            {
                var nombre = productos.TryGetValue(repetido.Key, out var p) ? p.Nombre : "un producto";
                errores.Add($"«{nombre}» está repetido: súmalo en una sola línea con la cantidad total.");
            }

            return errores;
        }

        public async Task<Resultado> AgregarComponenteAsync(int idCombo, LineaComponente linea, string usuario)
        {
            var combo = await _catalogo.ObtenerProductoAsync(idCombo);
            if (combo == null || !combo.EsCombo) return Resultado.Error("El combo no existe.");

            var errores = await ValidarComponentesAsync(idCombo, new List<LineaComponente> { linea });
            if (await _repo.ObtenerAsync(idCombo, linea.IdProducto) != null)
                errores.Add("Ese producto ya está en el combo: quítalo y vuelve a agregarlo con la cantidad correcta.");
            if (errores.Count > 0) return Resultado.Error(errores.ToArray());

            var componente = await _catalogo.ObtenerProductoAsync(linea.IdProducto);
            await _repo.AgregarAsync(new ComboProducto
            {
                IdProductoCombo = idCombo,
                IdProductoComponente = linea.IdProducto,
                Cantidad = linea.Cantidad,
                IdSabor = linea.IdSabor
            });
            await _auditoria.RegistrarAsync(usuario, "Modificar combo", "Producto", idCombo.ToString(),
                $"Agrega {linea.Cantidad} × {componente!.Nombre}");
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok($"«{componente.Nombre}» agregado al combo.");
        }

        public async Task<Resultado> QuitarComponenteAsync(int idCombo, int idComponente, string usuario)
        {
            var componente = await _repo.ObtenerAsync(idCombo, idComponente);
            if (componente == null) return Resultado.Error("Ese producto no está en el combo.");

            var restantes = await _repo.ListarComponentesAsync(idCombo);
            if (restantes.Count <= 1)
                return Resultado.Error("Un combo debe tener al menos un producto. Agrega otro antes de quitar este.");

            _repo.Eliminar(componente);
            await _auditoria.RegistrarAsync(usuario, "Modificar combo", "Producto", idCombo.ToString(),
                $"Quita el producto {idComponente}");
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok("Producto quitado del combo.");
        }
    }
}
