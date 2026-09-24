using Microsoft.AspNetCore.Identity;

namespace AlitasGoWeb.Seguridad
{
    // Mensajes de Identity en español para el personal del local.
    public class ErroresIdentityEspanol : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length) =>
            new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };
        public override IdentityError PasswordRequiresDigit() =>
            new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe tener al menos un número." };
        public override IdentityError PasswordRequiresUpper() =>
            new() { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe tener al menos una mayúscula." };
        public override IdentityError PasswordRequiresLower() =>
            new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe tener al menos una minúscula." };
        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe tener al menos un símbolo (por ejemplo ! o #)." };
        public override IdentityError DuplicateUserName(string userName) =>
            new() { Code = nameof(DuplicateUserName), Description = $"El usuario {userName} ya existe." };
        public override IdentityError DuplicateEmail(string email) =>
            new() { Code = nameof(DuplicateEmail), Description = $"El correo {email} ya está registrado." };
        public override IdentityError InvalidEmail(string? email) =>
            new() { Code = nameof(InvalidEmail), Description = $"El correo {email} no es válido." };
    }
}
