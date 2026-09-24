using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlitasGoWeb.Areas.Identity.Pages.Account
{
    // Página de Identity personalizada (en español). Reemplaza a la de la interfaz por defecto.
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ingresa tu correo.")]
            [EmailAddress(ErrorMessage = "Correo no válido.")]
            [Display(Name = "Correo")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ingresa tu contraseña.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            ReturnUrl = returnUrl ?? Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return Page();

            // Sin "recordarme": la caja es un equipo compartido. lockoutOnFailure = true (5 intentos → 15 min).
            var resultado = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Inicio de sesión de {Usuario}.", Input.Email);
                return LocalRedirect(returnUrl);   // LocalRedirect evita redirecciones abiertas
            }
            if (resultado.RequiresTwoFactor)
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
            if (resultado.IsLockedOut)
            {
                _logger.LogWarning("Cuenta bloqueada: {Usuario}.", Input.Email);
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada por varios intentos fallidos. Intenta de nuevo en 15 minutos.");
                return Page();
            }
            if (resultado.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Tu cuenta aún no está habilitada. Consulta con la administradora.");
                return Page();
            }

            // Mensaje genérico: no revela si el correo existe.
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return Page();
        }
    }
}
