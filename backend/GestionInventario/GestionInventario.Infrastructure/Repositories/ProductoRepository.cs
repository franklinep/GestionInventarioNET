using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionInventario.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly InventarioDbContext _db;

        public ProductoRepository(InventarioDbContext db)
        {
            _db = db;
        }

        public async Task<Producto> Add(Producto producto, CancellationToken ct)
        {
            await _db.Productos.AddAsync(producto, ct);
            await _db.SaveChangesAsync(ct);
            return producto;
        }

        public async Task<Producto?> GetById(short id, CancellationToken ct)
        {
            return await _db.Productos
                .FirstOrDefaultAsync(p => p.ProductoId == id && p.IsActivo, ct);
        }

        public async Task<IEnumerable<Producto>> GetByCategoria(short categoriaId, CancellationToken ct)
        {
            return await _db.Productos
                .Where(p => p.CategoriaId == categoriaId && p.IsActivo)
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Producto>> GetAllActivas(CancellationToken ct)
        {
            return await _db.Productos
                .Where(p => p.IsActivo)
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
        }

        public async Task Update(Producto producto, CancellationToken ct)
        {
            _db.Productos.Update(producto);
            await _db.SaveChangesAsync(ct);
        }

        public async Task Delete(short id, CancellationToken ct)
        {
            var producto = await _db.Productos.FindAsync(id, ct);
            if (producto != null)
            {
                producto.IsActivo = false;
                producto.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
