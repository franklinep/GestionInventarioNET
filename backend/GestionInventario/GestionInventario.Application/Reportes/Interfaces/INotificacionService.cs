using GestionInventario.Application.Reportes.DTOs;

namespace GestionInventario.Application.Reportes.Interfaces
{
    public interface INotificacionService
    {
        Task EnviarReporteStockBajoPorEmail(byte[] pdfBytes, ReporteStockBajoResponse reporte, CancellationToken ct = default);
    }
}
