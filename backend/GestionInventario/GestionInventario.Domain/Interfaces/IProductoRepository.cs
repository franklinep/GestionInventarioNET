using GestionInventario.Domain.Entities;

namespace GestionInventario.Domain.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto> Add(Producto producto, CancellationToken ct);
        Task<Producto?> GetById(short id, CancellationToken ct);
        Task<IEnumerable<Producto>> GetByCategoria(short categoriaId, CancellationToken ct);
        Task<IEnumerable<Producto>> GetAllActivas(CancellationToken ct);
        Task Update(Producto producto, CancellationToken ct);
        Task Delete(short id, CancellationToken ct);
    }
}
