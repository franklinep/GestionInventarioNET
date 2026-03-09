namespace GestionInventario.Application.Reportes.DTOs
{
    public record ProductoStockBajoDto
    {
        public short ProductoId { get; init; }
        public string Nombre { get; init; } = default!;
        public string CategoriaNombre { get; init; } = default!;
        public int StockActual { get; init; }
        public decimal Precio { get; init; }
        public string Nivel { get; init; } = default!;
        public DateTime UltimaActualizacion { get; init; }
    }
}
