using System.ComponentModel.DataAnnotations;

namespace GestionInventario.Application.Auth.DTOs
{
    public record LoginRequest(
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        string Correo,

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La contraseña es requerida")]
        string Password
    );
}
