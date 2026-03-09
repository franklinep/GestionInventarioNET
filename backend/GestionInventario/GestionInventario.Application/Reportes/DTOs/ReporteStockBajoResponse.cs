namespace GestionInventario.Application.Reportes.DTOs
{
    public record ReporteStockBajoResponse
    {
        public DateTime FechaGeneracion { get; init; }
        public int TotalProductos { get; init; }
        public int ProductosCriticos { get; init; } // stock < 5
        public decimal ValorAfectado { get; init; }
        public IEnumerable<ProductoStockBajoDto> Productos { get; init; } = [];
    }
}
