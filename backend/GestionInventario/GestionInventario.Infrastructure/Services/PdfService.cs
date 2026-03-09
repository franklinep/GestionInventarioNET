using DinkToPdf;
using DinkToPdf.Contracts;
using RazorLight;
using GestionInventario.Application.Reportes.Interfaces;

namespace GestionInventario.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _pdfConverter;
        private readonly RazorLightEngine _razorEngine;

        public PdfService(IConverter pdfConverter)
        {
            _pdfConverter = pdfConverter;

            var currentDirectory = Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(currentDirectory, "Templates");

            if (!Directory.Exists(templatePath))
            {
                throw new DirectoryNotFoundException($"No se encontró la carpeta Templates en: {currentDirectory} ni en {AppContext.BaseDirectory}");
            }

            _razorEngine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templatePath)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderizarTemplate<T>(string templateName, T model, CancellationToken ct = default)
        {
            var templatePath = $"{templateName}.cshtml";
            var html = await _razorEngine.CompileRenderAsync(templatePath, model);
            return html;
        }

        public Task<byte[]> GenerarPdfDesdeHtml(string html, CancellationToken ct = default)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
                DocumentTitle = "Reporte Stock Bajo"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
                FooterSettings = { FontSize = 9, Line = true, Center = "Reporte generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
            };

            var document = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var pdf = _pdfConverter.Convert(document);
            return Task.FromResult(pdf);
        }
    }
}
