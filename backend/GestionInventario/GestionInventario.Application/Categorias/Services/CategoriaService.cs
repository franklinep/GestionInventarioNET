using GestionInventario.Application.Categorias.DTOs;
using GestionInventario.Application.Categorias.Interfaces;
using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;

namespace GestionInventario.Application.Categorias.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepo;

        public CategoriaService(ICategoriaRepository categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        public async Task<IEnumerable<CategoriaResponse>> GetAllActivas(CancellationToken ct)
        {
            var categorias = await _categoriaRepo.GetAllActivas(ct);
            
            return categorias.Select(c => new CategoriaResponse
            {
                CategoriaId = c.CategoriaId,
                Nombre = c.Nombre,
                IsActivo = c.IsActivo,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CategoriaResponse?> GetById(short id, CancellationToken ct)
        {
            var categoria = await _categoriaRepo.GetById(id, ct);
            
            if (categoria == null)
                return null;
            
            return new CategoriaResponse
            {
                CategoriaId = categoria.CategoriaId,
                Nombre = categoria.Nombre,
                IsActivo = categoria.IsActivo,
                CreatedAt = categoria.CreatedAt,
                UpdatedAt = categoria.UpdatedAt
            };
        }

        public async Task<CategoriaResponse> Add(CreateCategoriaRequest request, CancellationToken ct)
        {
            var categoria = new Categoria
            {
                Nombre = request.Nombre.Trim(),
                IsActivo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var categoriaCreada = await _categoriaRepo.Add(categoria, ct);

            return new CategoriaResponse
            {
                CategoriaId = categoriaCreada.CategoriaId,
                Nombre = categoriaCreada.Nombre,
                IsActivo = categoriaCreada.IsActivo,
                CreatedAt = categoriaCreada.CreatedAt,
                UpdatedAt = categoriaCreada.UpdatedAt
            };
        }

        public async Task<CategoriaResponse> Update(short id, UpdateCategoriaRequest request, CancellationToken ct)
        {
            var categoria = await _categoriaRepo.GetById(id, ct);
            
            if (categoria == null)
                throw new InvalidOperationException("Categoría no encontrada");

            categoria.Nombre = request.Nombre.Trim();
            categoria.UpdatedAt = DateTime.UtcNow;

            await _categoriaRepo.Update(categoria, ct);

            return new CategoriaResponse
            {
                CategoriaId = categoria.CategoriaId,
                Nombre = categoria.Nombre,
                IsActivo = categoria.IsActivo,
                CreatedAt = categoria.CreatedAt,
                UpdatedAt = categoria.UpdatedAt
            };
        }

        public async Task Delete(short id, CancellationToken ct)
        {
            var categoria = await _categoriaRepo.GetById(id, ct);

            if (categoria == null)
                throw new InvalidOperationException("Categoría no encontrada");

            if (!categoria.IsActivo)
                throw new InvalidOperationException("La categoría ya está inactiva");

            if (await _categoriaRepo.TieneProductosAsociados(id, ct))
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados");

            await _categoriaRepo.Delete(id, ct);
        }
    }
}
