
namespace GestionInventario.Application.Productos.DTOs
{
    public record ProductoResponse
    {
        public short ProductoId { get; init; }
        public string Nombre { get; init; } = default!;
        public string? Descripcion { get; init; }
        public string? ImagenUrl { get; init; }
        public decimal Precio { get; init; }
        public int Stock { get; init; }
        public short CategoriaId { get; init; }
        public string CategoriaNombre { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public short? CreatedBy { get; init; }
        public short? UpdatedBy { get; init; }
    }
}
