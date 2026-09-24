using AlitasGoWeb.Seguridad;
using AlitasGoWeb.Services;
using AlitasGoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlitasGoWeb.Controllers
{
    // El estado de cada mesa deja de vivir "en la cabeza de Kevin".
    [Authorize(Policy = Politicas.TomarPedidos)]
    public class SalonController : ControladorBase
    {
        private readonly ICatalogoService _catalogo;
        private readonly IPedidoService _pedidos;

        public SalonController(ICatalogoService catalogo, IPedidoService pedidos)
        {
            _catalogo = catalogo;
            _pedidos = pedidos;
        }

        public async Task<IActionResult> Index() => View(await ArmarMesasAsync());

        // GET: /Salon/Mesas — bloque que se refresca en tiempo real
        public async Task<IActionResult> Mesas() => PartialView("_Mesas", await ArmarMesasAsync());

        private async Task<List<MesaSalonVM>> ArmarMesasAsync()
        {
            var mesas = await _catalogo.ListarMesasAsync();
            var activos = await _pedidos.ListarActivosSalonAsync();

            return mesas.Select(m => new MesaSalonVM
            {
                Mesa = m,
                PedidosActivos = activos.Where(p => p.PedidoLocal?.IdNroMesa == m.IdNroMesa).ToList()
            }).ToList();
        }
    }
}
