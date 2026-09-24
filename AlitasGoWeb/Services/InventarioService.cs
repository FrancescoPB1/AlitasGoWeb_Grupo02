using AlitasGoWeb.Data.Repositories;
using AlitasGoWeb.Models;
using AlitasGoWeb.Services.Dtos;

namespace AlitasGoWeb.Services
{
    // Cada docena servida se lleva su parte de alitas y de salsa: el consumo real lo marca el pedido.
    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _repo;
        private readonly ICatalogoRepository _catalogo;
        private readonly IAuditoriaService _auditoria;
        private readonly IUnidadDeTrabajo _uow;
        private readonly IReloj _reloj;

        public InventarioService(IInventarioRepository repo, ICatalogoRepository catalogo,
            IAuditoriaService auditoria, IUnidadDeTrabajo uow, IReloj reloj)
        {
            _repo = repo;
            _catalogo = catalogo;
            _auditoria = auditoria;
            _uow = uow;
            _reloj = reloj;
        }

        public async Task<List<ConsumoInsumo>> CalcularConsumoAsync(IEnumerable<DetallePedido> detalles)
        {
            var lista = detalles.ToList();
            if (lista.Count == 0) return new List<ConsumoInsumo>();

            var idsProducto = lista.Select(d => d.IdProducto).Distinct().ToList();
            var componentes = await _repo.ObtenerComponentesAsync(idsProducto);
            var idsConReceta = idsProducto.Concat(componentes.Select(c => c.IdProductoComponente)).Distinct();
            var recetas = await _repo.ObtenerRecetasAsync(idsConReceta);

            var acumulado = new Dictionary<int, decimal>();

            void Sumar(int idProducto, int? idSabor, decimal veces)
            {
                foreach (var r in recetas.Where(r => r.IdProducto == idProducto && (r.IdSabor == null || r.IdSabor == idSabor)))
                    acumulado[r.IdInsumo] = acumulado.GetValueOrDefault(r.IdInsumo) + r.CantidadUsada * veces;
            }

            foreach (var d in lista)
            {
                // Un combo consume lo que consumen sus componentes (con el sabor definido en el combo).
                var delCombo = componentes.Where(c => c.IdProductoCombo == d.IdProducto).ToList();
                if (delCombo.Count > 0)
                    foreach (var c in delCombo) Sumar(c.IdProductoComponente, c.IdSabor, c.Cantidad * d.Cantidad);
                else
                    Sumar(d.IdProducto, d.IdSabor, d.Cantidad);
            }

            return acumulado.Select(kv => new ConsumoInsumo(kv.Key, kv.Value)).ToList();
        }

        // Diferencia entre el consumo nuevo y el anterior (edición de un pedido).
        public static List<ConsumoInsumo> Diferencia(IEnumerable<ConsumoInsumo> nuevo, IEnumerable<ConsumoInsumo> anterior)
        {
            var neto = nuevo.ToDictionary(c => c.IdInsumo, c => c.Cantidad);
            foreach (var a in anterior)
                neto[a.IdInsumo] = neto.GetValueOrDefault(a.IdInsumo) - a.Cantidad;
            return neto.Where(kv => kv.Value != 0).Select(kv => new ConsumoInsumo(kv.Key, kv.Value)).ToList();
        }

        public async Task<List<string>> ValidarStockAsync(IEnumerable<ConsumoInsumo> consumo)
        {
            var requeridos = consumo.Where(c => c.Cantidad > 0).ToList();
            var errores = new List<string>();
            if (requeridos.Count == 0) return errores;

            var insumos = await _repo.ObtenerInsumosAsync(requeridos.Select(c => c.IdInsumo));
            foreach (var c in requeridos)
            {
                var insumo = insumos.FirstOrDefault(i => i.IdInsumo == c.IdInsumo);
                if (insumo != null && !insumo.HayStock(c.Cantidad))
                    errores.Add($"No hay stock suficiente de {insumo.Nombre}: se necesitan {c.Cantidad:0.##} {insumo.UnidadMedida} y quedan {insumo.StockActual:0.##}.");
            }
            return errores;
        }

        public Task DescontarAsync(IEnumerable<ConsumoInsumo> consumo, string motivo, Pedido? pedido, string usuario) =>
            MoverAsync(consumo, TiposMovimiento.Salida, motivo, pedido, usuario);

        public Task ReponerAsync(IEnumerable<ConsumoInsumo> consumo, string motivo, Pedido? pedido, string usuario) =>
            MoverAsync(consumo, TiposMovimiento.Entrada, motivo, pedido, usuario);

        // No guarda: el llamador confirma todo en su transacción.
        private async Task MoverAsync(IEnumerable<ConsumoInsumo> consumo, string tipo, string motivo, Pedido? pedido, string usuario)
        {
            var lista = consumo.Where(c => c.Cantidad > 0).ToList();
            if (lista.Count == 0) return;

            var insumos = await _repo.ObtenerInsumosAsync(lista.Select(c => c.IdInsumo));
            foreach (var c in lista)
            {
                var insumo = insumos.FirstOrDefault(i => i.IdInsumo == c.IdInsumo);
                if (insumo == null) continue;

                insumo.StockActual += tipo == TiposMovimiento.Entrada ? c.Cantidad : -c.Cantidad;
                await _repo.AgregarMovimientoAsync(new MovimientoInventario
                {
                    IdInsumo = insumo.IdInsumo,
                    Pedido = pedido,
                    Fecha = _reloj.Ahora,
                    Tipo = tipo,
                    Cantidad = c.Cantidad,
                    Motivo = motivo,
                    Usuario = usuario
                });
            }
        }

        public Task<List<Insumo>> ListarInsumosAsync() => _repo.ListarInsumosAsync();

        public Task<Insumo?> ObtenerInsumoAsync(int idInsumo) => _repo.ObtenerInsumoAsync(idInsumo);

        public async Task<Resultado> CrearInsumoAsync(Insumo insumo, string usuario)
        {
            insumo.Nombre = insumo.Nombre?.Trim() ?? string.Empty;
            if (await _repo.ExisteInsumoAsync(insumo.Nombre, 0))
                return Resultado.Error("Ya existe un insumo con ese nombre.");
            if (insumo.StockActual < 0) return Resultado.Error("El stock inicial no puede ser negativo.");

            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                var stockInicial = insumo.StockActual;
                insumo.StockActual = 0;
                await _repo.AgregarInsumoAsync(insumo);
                await _uow.GuardarCambiosAsync();   // genera el Id del insumo
                await ReponerAsync(new[] { new ConsumoInsumo(insumo.IdInsumo, stockInicial) }, "Stock inicial", null, usuario);
                await _auditoria.RegistrarAsync(usuario, "Crear insumo", "Insumo", insumo.IdInsumo.ToString(),
                    $"{insumo.Nombre} · stock inicial {stockInicial:0.##} {insumo.UnidadMedida}");
            });
            return Resultado.Ok($"Insumo «{insumo.Nombre}» registrado.");
        }

        public async Task<Resultado> EditarInsumoAsync(Insumo datos, string usuario)
        {
            var insumo = await _repo.ObtenerInsumoAsync(datos.IdInsumo);
            if (insumo == null) return Resultado.Error("El insumo no existe.");

            var nombre = datos.Nombre?.Trim() ?? string.Empty;
            if (await _repo.ExisteInsumoAsync(nombre, insumo.IdInsumo))
                return Resultado.Error("Ya existe un insumo con ese nombre.");

            var detalle = insumo.StockMinimo != datos.StockMinimo
                ? $"Stock mínimo {insumo.StockMinimo:0.##} → {datos.StockMinimo:0.##}"
                : "Datos generales";

            // El stock NO se edita a mano: solo cambia con compras, pedidos y anulaciones.
            insumo.Nombre = nombre;
            insumo.UnidadMedida = datos.UnidadMedida;
            insumo.StockMinimo = datos.StockMinimo;
            await _auditoria.RegistrarAsync(usuario, "Editar insumo", "Insumo", insumo.IdInsumo.ToString(), detalle);
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok("Insumo actualizado.");
        }

        public async Task<Resultado> RegistrarCompraAsync(List<LineaCompra> lineas, string usuario)
        {
            var validas = lineas.Where(l => l.IdInsumo > 0).ToList();
            if (validas.Count == 0) return Resultado.Error("Agrega al menos un insumo a la compra.");
            if (validas.Any(l => l.Cantidad <= 0)) return Resultado.Error("Las cantidades deben ser mayores que cero.");
            if (validas.Any(l => l.CostoUnitario < 0)) return Resultado.Error("El costo unitario no puede ser negativo.");

            var insumos = await _repo.ObtenerInsumosAsync(validas.Select(l => l.IdInsumo));
            if (insumos.Count != validas.Select(l => l.IdInsumo).Distinct().Count())
                return Resultado.Error("Uno de los insumos no existe.");

            var compra = new CompraInsumos { Fecha = _reloj.Ahora, Usuario = usuario };
            foreach (var l in validas)
            {
                compra.Detalles.Add(new DetalleCompraInsumo
                {
                    InsumoId = l.IdInsumo,
                    Cantidad = l.Cantidad,
                    CostoUnitario = l.CostoUnitario,
                    Subtotal = Math.Round(l.Cantidad * l.CostoUnitario, 2)
                });
            }
            compra.Total = compra.Detalles.Sum(d => d.Subtotal);

            var entradas = validas.GroupBy(l => l.IdInsumo)
                                  .Select(g => new ConsumoInsumo(g.Key, g.Sum(l => l.Cantidad)));

            await _uow.EjecutarEnTransaccionAsync(async () =>
            {
                await _repo.AgregarCompraAsync(compra);
                await ReponerAsync(entradas, "Compra de insumos", null, usuario);
                await _auditoria.RegistrarAsync(usuario, "Registrar compra", "CompraInsumos", null,
                    $"{validas.Count} línea(s) · total S/ {compra.Total:0.00}");
            });
            return Resultado.Ok("Compra registrada: el stock se actualizó.");
        }

        public Task<List<MovimientoInventario>> ListarMovimientosAsync(int? idInsumo) =>
            _repo.ListarMovimientosAsync(idInsumo, 300);

        public Task<List<Receta>> ListarRecetaAsync(int idProducto) => _repo.ListarRecetaDeProductoAsync(idProducto);

        public async Task<Resultado> AgregarLineaRecetaAsync(int idProducto, int idInsumo, int? idSabor, decimal cantidad, string usuario)
        {
            var producto = await _catalogo.ObtenerProductoAsync(idProducto);
            if (producto == null) return Resultado.Error("El producto no existe.");
            var insumo = await _repo.ObtenerInsumoAsync(idInsumo);
            if (insumo == null) return Resultado.Error("Selecciona un insumo válido.");
            if (cantidad <= 0) return Resultado.Error("La cantidad usada debe ser mayor que cero.");
            if (idSabor.HasValue && await _catalogo.ObtenerSaborAsync(idSabor.Value) == null)
                return Resultado.Error("Selecciona un sabor válido.");

            await _repo.AgregarRecetaAsync(new Receta
            {
                IdProducto = idProducto,
                IdInsumo = idInsumo,
                IdSabor = idSabor,
                CantidadUsada = cantidad,
                UnidadMedida = insumo.UnidadMedida,
                Activo = true
            });
            await _auditoria.RegistrarAsync(usuario, "Modificar receta", "Producto", idProducto.ToString(),
                $"Agrega {cantidad:0.##} {insumo.UnidadMedida} de {insumo.Nombre}");
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok("Línea agregada a la receta.");
        }

        public async Task<Resultado> QuitarLineaRecetaAsync(int idReceta, string usuario)
        {
            var receta = await _repo.ObtenerRecetaAsync(idReceta);
            if (receta == null) return Resultado.Error("La línea de receta no existe.");

            _repo.EliminarReceta(receta);
            await _auditoria.RegistrarAsync(usuario, "Modificar receta", "Producto", receta.IdProducto.ToString(),
                $"Quita la línea {idReceta} (insumo {receta.IdInsumo})");
            await _uow.GuardarCambiosAsync();
            return Resultado.Ok("Línea quitada de la receta.");
        }
    }
}
