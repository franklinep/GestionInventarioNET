namespace GestionInventario.Application.Productos.DTOs
{
    public record ProductoFiltroRequest
    {
        public string? Search { get; init; }
        public List<short>? CategoriaIds { get; init; }
        public int? StockMaximo { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
