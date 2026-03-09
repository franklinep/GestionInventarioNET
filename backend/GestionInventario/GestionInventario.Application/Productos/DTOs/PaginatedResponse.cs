namespace GestionInventario.Application.Productos.DTOs
{
    public record PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; init; } = [];
        public int TotalItems { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
