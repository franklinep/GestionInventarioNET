using Microsoft.AspNetCore.Http;

namespace GestionInventario.Application.Productos.Interfaces
{
    public interface IImagenService
    {
        Task<string> GuardarImagen(IFormFile imagen, short productoId, CancellationToken ct);
        Task EliminarImagen(string? imagenUrl, CancellationToken ct);
    }
}
