using GestionInventario.Application.Reportes.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionInventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        /// <summary>
        /// Obtiene reporte JSON de productos con stock bajo
        /// </summary>
        [HttpGet("stock-bajo")]
        [Authorize] // Admin y Empleado pueden ver
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetReporteStockBajo([FromQuery] int umbral = 5, CancellationToken ct = default)
        {
            try
            {
                var reporte = await _reporteService.GenerarReporteStockBajo(umbral, ct);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Genera y descarga PDF de productos con stock bajo
        /// </summary>
        [HttpGet("stock-bajo/pdf")]
        [Authorize] // Admin y Empleado pueden descargar
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DescargarPdfStockBajo([FromQuery] int umbral = 5, CancellationToken ct = default)
        {
            try
            {
                var pdf = await _reporteService.GenerarPdfStockBajo(umbral, ct);
                var nombreArchivo = $"ReporteStockBajo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(pdf, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar PDF", detail = ex.Message });
            }
        }

        /// <summary>
        /// Envía el reporte PDF de stock bajo por email al administrador
        /// </summary>
        [HttpPost("stock-bajo/enviar-email")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EnviarReportePorEmail([FromQuery] int umbral = 5, CancellationToken ct = default)
        {
            try
            {
                await _reporteService.EnviarReporteStockBajoPorEmail(umbral, ct);
                return Ok(new { message = "Reporte enviado exitosamente al administrador" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al enviar el reporte por email", detail = ex.Message });
            }
        }
    }
}
    