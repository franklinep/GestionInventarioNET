using GestionInventario.Application.Productos.DTOs;
using GestionInventario.Application.Productos.Interfaces;
using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GestionInventario.Application.Productos.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly IImagenService _imagenService;

        public ProductoService(
            IProductoRepository productoRepo, 
            ICategoriaRepository categoriaRepo,
            IImagenService imagenService)
        {
            _productoRepo = productoRepo;
            _categoriaRepo = categoriaRepo;
            _imagenService = imagenService;
        }

        public async Task<IEnumerable<ProductoResponse>> GetAllActivas(CancellationToken ct)
        {
            var productos = await _productoRepo.GetAllActivas(ct);
            var responses = new List<ProductoResponse>();

            foreach (var producto in productos)
            {
                var categoria = await _categoriaRepo.GetById(producto.CategoriaId, ct);

                responses.Add(new ProductoResponse
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    ImagenUrl = producto.ImagenUrl,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    CategoriaId = producto.CategoriaId,
                    CategoriaNombre = categoria?.Nombre ?? "Sin categoría",
                    CreatedAt = producto.CreatedAt,
                    UpdatedAt = producto.UpdatedAt,
                    CreatedBy = producto.CreatedBy,
                    UpdatedBy = producto.UpdatedBy
                });
            }

            return responses;
        }

        public async Task<ProductoResponse?> GetById(short id, CancellationToken ct)
        {
            var producto = await _productoRepo.GetById(id, ct);

            if (producto == null)
                return null;

            var categoria = await _categoriaRepo.GetById(producto.CategoriaId, ct);

            return new ProductoResponse
            {
                ProductoId = producto.ProductoId,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                ImagenUrl = producto.ImagenUrl,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = categoria?.Nombre ?? "Sin categoría",
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt,
                CreatedBy = producto.CreatedBy,
                UpdatedBy = producto.UpdatedBy
            };
        }

        public async Task<ProductoResponse> Add(CreateProductoRequest request, IFormFile? imagen, short usuarioId, CancellationToken ct)
        {
            var categoria = await _categoriaRepo.GetById(request.CategoriaId, ct);
            if (categoria == null)
                throw new InvalidOperationException("La categoría especificada no existe");

            if (request.Precio <= 0)
                throw new InvalidOperationException("El precio debe ser mayor a 0");

            if (request.Stock < 0)
                throw new InvalidOperationException("El stock no puede ser negativo");

            var producto = new Producto
            {
                Nombre = request.Nombre.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                IsActivo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = usuarioId,
                UpdatedBy = usuarioId
            };

            var productoCreado = await _productoRepo.Add(producto, ct);

            if (imagen != null && imagen.Length > 0)
            {
                productoCreado.ImagenUrl = await _imagenService.GuardarImagen(imagen, productoCreado.ProductoId, ct);
                await _productoRepo.Update(productoCreado, ct);
            }

            return new ProductoResponse
            {
                ProductoId = productoCreado.ProductoId,
                Nombre = productoCreado.Nombre,
                Descripcion = productoCreado.Descripcion,
                ImagenUrl = productoCreado.ImagenUrl,
                Precio = productoCreado.Precio,
                Stock = productoCreado.Stock,
                CategoriaId = productoCreado.CategoriaId,
                CategoriaNombre = categoria.Nombre,
                CreatedAt = productoCreado.CreatedAt,
                UpdatedAt = productoCreado.UpdatedAt,
                CreatedBy = productoCreado.CreatedBy,
                UpdatedBy = productoCreado.UpdatedBy
            };
        }

        public async Task<ProductoResponse> Update(short id, UpdateProductoRequest request, IFormFile? imagen, short usuarioId, CancellationToken ct)
        {
            var producto = await _productoRepo.GetById(id, ct);
            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado");

            var categoria = await _categoriaRepo.GetById(request.CategoriaId, ct);
            if (categoria == null)
                throw new InvalidOperationException("La categoría especificada no existe");

            if (request.Precio <= 0)
                throw new InvalidOperationException("El precio debe ser mayor a 0");

            if (request.Stock < 0)
                throw new InvalidOperationException("El stock no puede ser negativo");

            producto.Nombre = request.Nombre.Trim();
            producto.Descripcion = request.Descripcion?.Trim();
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.CategoriaId = request.CategoriaId;
            producto.UpdatedAt = DateTime.UtcNow;
            producto.UpdatedBy = usuarioId;

            if (imagen != null && imagen.Length > 0)
            {
                await _imagenService.EliminarImagen(producto.ImagenUrl, ct);
                producto.ImagenUrl = await _imagenService.GuardarImagen(imagen, producto.ProductoId, ct);
            }

            await _productoRepo.Update(producto, ct);

            return new ProductoResponse
            {
                ProductoId = producto.ProductoId,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                ImagenUrl = producto.ImagenUrl,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = categoria.Nombre,
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt,
                CreatedBy = producto.CreatedBy,
                UpdatedBy = producto.UpdatedBy
            };
        }

        public async Task Delete(short id, CancellationToken ct)
        {
            var producto = await _productoRepo.GetById(id, ct);
            
            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado");
            
            if (!producto.IsActivo)
                throw new InvalidOperationException("El producto ya está inactivo");

            await _productoRepo.Delete(id, ct);
        }

        public async Task<IEnumerable<ProductoResponse>> GetByCategoria(short categoriaId, CancellationToken ct)
        {
            var categoria = await _categoriaRepo.GetById(categoriaId, ct);
            if (categoria == null)
                throw new InvalidOperationException("La categoría especificada no existe");

            var productos = await _productoRepo.GetByCategoria(categoriaId, ct);

            return productos.Select(p => new ProductoResponse
            {
                ProductoId = p.ProductoId,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                ImagenUrl = p.ImagenUrl,
                Precio = p.Precio,
                Stock = p.Stock,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = categoria.Nombre,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy
            });
        }

        public async Task<PaginatedResponse<ProductoResponse>> GetPaginated(ProductoFiltroRequest filtro, CancellationToken ct)
        {
            var productos = await _productoRepo.GetAllActivas(ct);
            var query = productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Search))
                query = query.Where(p => p.Nombre.Contains(filtro.Search, StringComparison.OrdinalIgnoreCase));

            if (filtro.CategoriaIds?.Count > 0)
                query = query.Where(p => filtro.CategoriaIds.Contains(p.CategoriaId));

            if (filtro.StockMaximo.HasValue)
                query = query.Where(p => p.Stock <= filtro.StockMaximo.Value);

            var totalItems = query.Count();
            var items = query
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToList();

            var responses = new List<ProductoResponse>();
            foreach (var p in items)
            {
                var categoria = await _categoriaRepo.GetById(p.CategoriaId, ct);
                responses.Add(new ProductoResponse
                {
                    ProductoId = p.ProductoId,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    ImagenUrl = p.ImagenUrl,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = categoria?.Nombre ?? "Sin categoría",
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy
                });
            }

            return new PaginatedResponse<ProductoResponse>
            {
                Items = responses,
                TotalItems = totalItems,
                Page = filtro.Page,
                PageSize = filtro.PageSize
            };
        }
    }
}
