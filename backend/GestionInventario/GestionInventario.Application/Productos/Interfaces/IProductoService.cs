using GestionInventario.Application.Productos.DTOs;
using Microsoft.AspNetCore.Http;

namespace GestionInventario.Application.Productos.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoResponse>> GetAllActivas(CancellationToken ct);
        Task<PaginatedResponse<ProductoResponse>> GetPaginated(ProductoFiltroRequest filtro, CancellationToken ct);
        Task<ProductoResponse?> GetById(short id, CancellationToken ct);
        Task<ProductoResponse> Add(CreateProductoRequest request, IFormFile? imagen, short usuarioId, CancellationToken ct);
        Task<ProductoResponse> Update(short id, UpdateProductoRequest request, IFormFile? imagen, short usuarioId, CancellationToken ct);
        Task Delete(short id, CancellationToken ct);
        Task<IEnumerable<ProductoResponse>> GetByCategoria(short categoriaId, CancellationToken ct);
    }
}
