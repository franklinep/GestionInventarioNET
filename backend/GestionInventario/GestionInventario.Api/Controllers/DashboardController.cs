using GestionInventario.Application.Dashboard.DTOs;
using GestionInventario.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionInventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        [Authorize]
        [ProducesResponseType(typeof(DashboardStatsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStats(CancellationToken ct)
        {
            try
            {
                var stats = await _dashboardService.GetStats(ct);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}
