using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlitasGoWeb.Areas.Identity.Pages.Account
{
    // Segundo factor (app autenticadora). Al ingresar por aquí, Identity agrega el claim amr = "mfa",
    // que exigen las políticas SoloAdmin y AnularPedido.
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(SignInManager<IdentityUser> signInManager, ILogger<LoginWith2faModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ingresa el código.")]
            [StringLength(7, MinimumLength = 6, ErrorMessage = "El código tiene 6 dígitos.")]
            [DataType(DataType.Text)]
            [Display(Name = "Código de la app autenticadora")]
            public string TwoFactorCode { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            // Solo se llega aquí después de validar la contraseña.
            var usuario = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (usuario == null) return RedirectToPage("./Login");

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return Page();

            var usuario = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (usuario == null) return RedirectToPage("./Login");

            var codigo = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            // rememberClient: false → el código se pide en cada inicio de sesión del Administrador.
            var resultado = await _signInManager.TwoFactorAuthenticatorSignInAsync(codigo, isPersistent: false, rememberClient: false);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("{Usuario} ingresó con segundo factor.", usuario.UserName);
                return LocalRedirect(returnUrl);
            }
            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada por varios intentos fallidos. Intenta en 15 minutos.");
                return Page();
            }

            _logger.LogWarning("Código 2FA inválido para {Usuario}.", usuario.UserName);
            ModelState.AddModelError(string.Empty, "Código incorrecto.");
            return Page();
        }
    }
}
