using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionInventario.Application.Productos.DTOs
{
    public record UpdateProductoRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Nombre { get; init; } = default!;

        [StringLength(1000)]
        public string? Descripcion { get; init; }

        /// <summary>
        /// Nueva imagen del producto (opcional, máx 5MB, formatos: jpg, jpeg, png, gif, webp)
        /// </summary>
        public IFormFile? Imagen { get; init; }

        [Required]
        [Range(0.01, 999999999999.99)]
        public decimal Precio { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; init; }

        [Required]
        public short CategoriaId { get; init; }
    }
}
