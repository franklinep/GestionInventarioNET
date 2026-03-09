namespace GestionInventario.Application.Categorias.DTOs
{
    public record CategoriaResponse
    {
        public short CategoriaId { get; init; }
        public string Nombre { get; init; } = default!;
        public bool IsActivo { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
