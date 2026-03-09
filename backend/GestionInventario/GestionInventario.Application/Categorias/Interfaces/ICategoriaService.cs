using GestionInventario.Application.Categorias.DTOs;

namespace GestionInventario.Application.Categorias.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaResponse>> GetAllActivas(CancellationToken ct);
        Task<CategoriaResponse?> GetById(short id, CancellationToken ct);
        Task<CategoriaResponse> Add(CreateCategoriaRequest request, CancellationToken ct);
        Task<CategoriaResponse> Update(short id, UpdateCategoriaRequest request, CancellationToken ct);
        Task Delete(short id, CancellationToken ct);
    }
}
