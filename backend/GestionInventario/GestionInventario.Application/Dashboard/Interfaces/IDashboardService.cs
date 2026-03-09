using GestionInventario.Application.Dashboard.DTOs;

namespace GestionInventario.Application.Dashboard.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsResponse> GetStats(CancellationToken ct);
    }
}
