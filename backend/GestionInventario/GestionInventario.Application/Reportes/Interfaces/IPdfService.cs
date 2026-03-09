namespace GestionInventario.Application.Reportes.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerarPdfDesdeHtml(string html, CancellationToken ct = default);
        Task<string> RenderizarTemplate<T>(string templateName, T model, CancellationToken ct = default);
    }
}
