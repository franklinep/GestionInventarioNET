using GestionInventario.Application.Reportes.DTOs;

namespace GestionInventario.Application.Reportes.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteStockBajoResponse> GenerarReporteStockBajo(int umbral = 5, CancellationToken ct = default);
        Task<byte[]> GenerarPdfStockBajo(int umbral = 5, CancellationToken ct = default);
        Task EnviarReporteStockBajoPorEmail(int umbral = 5, CancellationToken ct = default);
    }
}
