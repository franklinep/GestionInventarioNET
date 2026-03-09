using GestionInventario.Application.Productos.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GestionInventario.Infrastructure.Services
{
    public class ImagenService : IImagenService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _imagenesPath = "images/productos";
        private readonly string[] _extensionesPermitidas = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private const long MaxFileSize = 5 * 1024 * 1024;

        public ImagenService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> GuardarImagen(IFormFile imagen, short productoId, CancellationToken ct)
        {
            if (imagen == null || imagen.Length == 0)
                throw new InvalidOperationException("No se proporcionó una imagen válida");

            if (imagen.Length > MaxFileSize)
                throw new InvalidOperationException("La imagen no puede superar los 5 MB");

            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
            if (!_extensionesPermitidas.Contains(extension))
                throw new InvalidOperationException($"Extensión no permitida. Use: {string.Join(", ", _extensionesPermitidas)}");

            var uploadsPath = Path.Combine(_environment.WebRootPath, _imagenesPath);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var nombreArchivo = $"{productoId}_{Guid.NewGuid():N}{extension}";
            var rutaCompleta = Path.Combine(uploadsPath, nombreArchivo);

            await using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await imagen.CopyToAsync(stream, ct);

            return $"/{_imagenesPath}/{nombreArchivo}";
        }

        public Task EliminarImagen(string? imagenUrl, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(imagenUrl))
                return Task.CompletedTask;

            try
            {
                var rutaRelativa = imagenUrl.TrimStart('/');
                var rutaCompleta = Path.Combine(_environment.WebRootPath, rutaRelativa);

                if (File.Exists(rutaCompleta))
                    File.Delete(rutaCompleta);
            }
            catch
            {
            }

            return Task.CompletedTask;
        }
    }
}
