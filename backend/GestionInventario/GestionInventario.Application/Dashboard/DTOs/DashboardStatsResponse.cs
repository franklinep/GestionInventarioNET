namespace GestionInventario.Application.Dashboard.DTOs
{
    public record DashboardStatsResponse
    {
        public int TotalProductos { get; init; }
        public int StockTotal { get; init; }
        public int TotalCategorias { get; init; }
        public int ProductosCriticos { get; init; }
    }
}
