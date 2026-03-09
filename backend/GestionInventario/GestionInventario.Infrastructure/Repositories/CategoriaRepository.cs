using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionInventario.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly InventarioDbContext _db;

        public CategoriaRepository(InventarioDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Categoria>> GetAllActivas(CancellationToken ct)
        {
            return await _db.Categorias
                .Where(c => c.IsActivo)
                .OrderBy(c => c.Nombre)
                .ToListAsync(ct);
        }

        public async Task<Categoria?> GetById(short id, CancellationToken ct)
        {
            return await _db.Categorias
                .FirstOrDefaultAsync(c => c.CategoriaId == id && c.IsActivo, ct);
        }

        public async Task<Categoria> Add(Categoria categoria, CancellationToken ct)
        {
            await _db.Categorias.AddAsync(categoria, ct);
            await _db.SaveChangesAsync(ct);
            return categoria;
        }

        public async Task Update(Categoria categoria, CancellationToken ct)
        {
            _db.Categorias.Update(categoria);
            await _db.SaveChangesAsync(ct);
        }

        public async Task Delete(short id, CancellationToken ct)
        {
            var categoria = await _db.Categorias.FindAsync(id, ct);
            if (categoria != null)
            {
                categoria.IsActivo = false;
                categoria.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
