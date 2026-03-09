using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using GestionInventario.Application.Reportes.DTOs;
using GestionInventario.Application.Reportes.Interfaces;

namespace GestionInventario.Infrastructure.Services
{
    public class EmailNotificationService : INotificacionService
    {
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarReporteStockBajoPorEmail(byte[] pdfBytes, ReporteStockBajoResponse reporte, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sistema Inventario", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("Administrador", _configuration["Email:AdminTo"]));
            message.Subject = $"📊 Reporte de Inventario Bajo - {reporte.FechaGeneracion:dd/MM/yyyy}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <h2>📊 Reporte de Inventario Bajo</h2>
                <p>Generado el {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}</p>
                <ul>
                    <li><strong>Total Productos:</strong> {reporte.TotalProductos}</li>
                    <li><strong>Productos Críticos:</strong> {reporte.ProductosCriticos}</li>
                    <li><strong>Valor Afectado:</strong> ${reporte.ValorAfectado:N2}</li>
                </ul>
                <p>Se adjunta el reporte completo en formato PDF.</p>"
            };

            var nombreArchivo = $"ReporteStockBajo_{reporte.FechaGeneracion:yyyyMMdd_HHmmss}.pdf";
            bodyBuilder.Attachments.Add(nombreArchivo, pdfBytes, new ContentType("application", "pdf"));
            message.Body = bodyBuilder.ToMessageBody();

            await EnviarEmailAsync(message, ct);
        }

        private async Task EnviarEmailAsync(MimeMessage message, CancellationToken ct)
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(
                _configuration["Email:SmtpHost"],
                int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls,
                ct);

            await client.AuthenticateAsync(
                _configuration["Email:Username"],
                _configuration["Email:Password"],
                ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
