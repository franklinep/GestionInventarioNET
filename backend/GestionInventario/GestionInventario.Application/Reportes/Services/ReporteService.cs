using GestionInventario.Application.Reportes.DTOs;
using GestionInventario.Application.Reportes.Interfaces;
using GestionInventario.Domain.Interfaces;

namespace GestionInventario.Application.Reportes.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly IPdfService _pdfService;
        private readonly INotificacionService _notificacionService;

        public ReporteService(
            IProductoRepository productoRepo, 
            ICategoriaRepository categoriaRepo,
            IPdfService pdfService,
            INotificacionService notificacionService)
        {
            _productoRepo = productoRepo;
            _categoriaRepo = categoriaRepo;
            _pdfService = pdfService;
            _notificacionService = notificacionService;
        }

        public async Task<ReporteStockBajoResponse> GenerarReporteStockBajo(int umbral = 5, CancellationToken ct = default)
        {
            var productos = await _productoRepo.GetAllActivas(ct);
            var productosBajoStock = new List<ProductoStockBajoDto>();

            foreach (var producto in productos.Where(p => p.Stock <= umbral))
            {
                var categoria = await _categoriaRepo.GetById(producto.CategoriaId, ct);
                
                productosBajoStock.Add(new ProductoStockBajoDto
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    CategoriaNombre = categoria?.Nombre ?? "Sin categoría",
                    StockActual = producto.Stock,
                    Precio = producto.Precio,
                    Nivel = producto.Stock < 5 ? "Crítico" : "Bajo",
                    UltimaActualizacion = producto.UpdatedAt
                });
            }

            return new ReporteStockBajoResponse
            {
                FechaGeneracion = DateTime.UtcNow,
                TotalProductos = productosBajoStock.Count,
                ProductosCriticos = productosBajoStock.Count(p => p.Nivel == "Crítico"),
                ValorAfectado = productosBajoStock.Sum(p => p.Precio * p.StockActual),
                Productos = productosBajoStock.OrderBy(p => p.StockActual)
            };
        }

        public async Task<byte[]> GenerarPdfStockBajo(int umbral = 5, CancellationToken ct = default)
        {
            var reporte = await GenerarReporteStockBajo(umbral, ct);
            var html = await _pdfService.RenderizarTemplate("ReporteStockBajo", reporte, ct);
            var pdf = await _pdfService.GenerarPdfDesdeHtml(html, ct);
            return pdf;
        }

        public async Task EnviarReporteStockBajoPorEmail(int umbral = 5, CancellationToken ct = default)
        {
            var reporte = await GenerarReporteStockBajo(umbral, ct);
            var html = await _pdfService.RenderizarTemplate("ReporteStockBajo", reporte, ct);
            var pdfBytes = await _pdfService.GenerarPdfDesdeHtml(html, ct);

            await _notificacionService.EnviarReporteStockBajoPorEmail(pdfBytes, reporte, ct);
        }
    }
}
