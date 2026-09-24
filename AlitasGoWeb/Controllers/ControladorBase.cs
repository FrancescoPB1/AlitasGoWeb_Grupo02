using AlitasGoWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlitasGoWeb.Controllers
{
    public abstract class ControladorBase : Controller
    {
        protected string UsuarioActual => User.Identity?.Name ?? "(anónimo)";

        protected bool EsAjax => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // Muestra el resultado en las alertas del layout (_AlertasPartial).
        protected void Notificar(Resultado r)
        {
            if (r.Exito) TempData["SuccessMessage"] = r.Mensaje;
            else TempData["ErrorMessage"] = string.Join(" ", r.Errores);
        }

        protected static void CopiarErrores(Resultado r, ModelStateDictionary modelState)
        {
            foreach (var e in r.Errores) modelState.AddModelError(string.Empty, e);
        }
    }
}
