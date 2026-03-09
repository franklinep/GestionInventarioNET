using GestionInventario.Application.Dashboard.DTOs;
using GestionInventario.Application.Dashboard.Interfaces;
using GestionInventario.Domain.Interfaces;

namespace GestionInventario.Application.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private const int UmbralCritico = 5;

        public DashboardService(IProductoRepository productoRepo, ICategoriaRepository categoriaRepo)
        {
            _productoRepo = productoRepo;
            _categoriaRepo = categoriaRepo;
        }

        public async Task<DashboardStatsResponse> GetStats(CancellationToken ct)
        {
            var productos = await _productoRepo.GetAllActivas(ct);
            var categorias = await _categoriaRepo.GetAllActivas(ct);

            var productosList = productos.ToList();

            return new DashboardStatsResponse
            {
                TotalProductos = productosList.Count,
                StockTotal = productosList.Sum(p => p.Stock),
                TotalCategorias = categorias.Count(),
                ProductosCriticos = productosList.Count(p => p.Stock <= UmbralCritico)
            };
        }
    }
}
