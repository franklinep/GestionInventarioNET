using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionInventario.Application.Productos.DTOs
{
    public class CreateProductoRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(150, MinimumLength = 3)]
        public string Nombre { get; init; } = default!;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Descripcion { get; init; }

        /// <summary>
        /// Archivo de imagen del producto (máx 5MB, formatos: jpg, jpeg, png, gif, webp)
        /// </summary>
        public IFormFile? Imagen { get; init; }

        [Required]
        [Range(0.0001, 999999999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; init; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; init; }

        [Required]
        public short CategoriaId { get; init; }
    }
}
