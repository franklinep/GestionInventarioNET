using FluentAssertions;
using GestionInventario.Application.Productos.DTOs;
using GestionInventario.Application.Productos.Interfaces;
using GestionInventario.Application.Productos.Services;
using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestionInventario.Test.Productos
{
    public class ProductoServiceTests
    {
        private readonly Mock<IProductoRepository> _productoRepoMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
        private readonly Mock<IImagenService> _imagenServiceMock;
        private readonly ProductoService _sut;

        public ProductoServiceTests()
        {
            _productoRepoMock = new Mock<IProductoRepository>();
            _categoriaRepoMock = new Mock<ICategoriaRepository>();
            _imagenServiceMock = new Mock<IImagenService>();

            _sut = new ProductoService(
                _productoRepoMock.Object,
                _categoriaRepoMock.Object,
                _imagenServiceMock.Object);
        }

        [Fact]
        public async Task GetAllActivas_RetornaTodosLosProductosActivos()
        {
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 10, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Producto B", Stock = 5, Precio = 200, CategoriaId = 1 }
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Electrónicos" };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            var resultado = await _sut.GetAllActivas(CancellationToken.None);

            resultado.Should().HaveCount(2);
            resultado.First().CategoriaNombre.Should().Be("Electrónicos");
        }

        [Fact]
        public async Task GetById_ProductoExiste_RetornaProducto()
        {
            var producto = new Producto
            {
                ProductoId = 1,
                Nombre = "Laptop",
                Precio = 1500,
                Stock = 10,
                CategoriaId = 1
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Electrónicos" };

            _productoRepoMock.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(producto);
            _categoriaRepoMock.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            var resultado = await _sut.GetById(1, CancellationToken.None);

            resultado.Should().NotBeNull();
            resultado!.Nombre.Should().Be("Laptop");
            resultado.CategoriaNombre.Should().Be("Electrónicos");
        }

        [Fact]
        public async Task GetById_ProductoNoExiste_RetornaNull()
        {
            _productoRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Producto?)null);

            var resultado = await _sut.GetById(999, CancellationToken.None);

            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Add_DatosValidos_CreaProducto()
        {
            var request = new CreateProductoRequest
            {
                Nombre = "Nuevo Producto",
                Precio = 100,
                Stock = 10,
                CategoriaId = 1
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };
            var productoCreado = new Producto
            {
                ProductoId = 1,
                Nombre = "Nuevo Producto",
                Precio = 100,
                Stock = 10,
                CategoriaId = 1
            };

            _categoriaRepoMock.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);
            _productoRepoMock.Setup(r => r.Add(It.IsAny<Producto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productoCreado);

            var resultado = await _sut.Add(request, imagen: null, usuarioId: 1, CancellationToken.None);

            resultado.Nombre.Should().Be("Nuevo Producto");
            resultado.Precio.Should().Be(100);
        }

        [Fact]
        public async Task Add_CategoriaNoExiste_LanzaExcepcion()
        {
            var request = new CreateProductoRequest
            {
                Nombre = "Producto",
                Precio = 100,
                Stock = 10,
                CategoriaId = 999
            };

            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Categoria?)null);

            var act = () => _sut.Add(request, imagen: null, usuarioId: 1, CancellationToken.None);


            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("La categoría especificada no existe");
        }

        [Fact]
        public async Task Add_PrecioNegativo_LanzaExcepcion()
        {
            var request = new CreateProductoRequest
            {
                Nombre = "Producto",
                Precio = -10,
                Stock = 10,
                CategoriaId = 1
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

            _categoriaRepoMock.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            var act = () => _sut.Add(request, imagen: null, usuarioId: 1, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El precio debe ser mayor a 0");
        }

        [Fact]
        public async Task Add_StockNegativo_LanzaExcepcion()
        {
            var request = new CreateProductoRequest
            {
                Nombre = "Producto",
                Precio = 100,
                Stock = -5,
                CategoriaId = 1
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

            _categoriaRepoMock.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            var act = () => _sut.Add(request, imagen: null, usuarioId: 1, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El stock no puede ser negativo");
        }

        [Fact]
        public async Task Update_ProductoNoExiste_LanzaExcepcion()
        {
            var request = new UpdateProductoRequest
            {
                Nombre = "Actualizado",
                Precio = 150,
                Stock = 20,
                CategoriaId = 1
            };

            _productoRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Producto?)null);

            var act = () => _sut.Update(999, request, imagen: null, usuarioId: 1, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Producto no encontrado");
        }
    }
}
