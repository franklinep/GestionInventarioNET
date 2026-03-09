using System.ComponentModel.DataAnnotations;

namespace GestionInventario.Application.Categorias.DTOs
{
    public record UpdateCategoriaRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres")]
        public string Nombre { get; init; } = default!;
    }
}
