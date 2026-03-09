using GestionInventario.Domain.Entities;

namespace GestionInventario.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllActivas(CancellationToken ct);
        Task<Categoria?> GetById(short id, CancellationToken ct);
        Task<Categoria> Add(Categoria categoria, CancellationToken ct);
        Task Delete(short id, CancellationToken ct);
        Task Update(Categoria categoria, CancellationToken ct);
    }
}
