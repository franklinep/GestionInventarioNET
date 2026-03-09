using FluentAssertions;
using GestionInventario.Application.Reportes.DTOs;
using GestionInventario.Application.Reportes.Interfaces;
using GestionInventario.Application.Reportes.Services;
using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestionInventario.Test.Reportes
{
    public class ReporteServiceTests
    {
        private readonly Mock<IProductoRepository> _productoRepoMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
        private readonly Mock<IPdfService> _pdfServiceMock;
        private readonly Mock<INotificacionService> _notificacionServiceMock;
        private readonly ReporteService _sut;

        public ReporteServiceTests()
        {
            _productoRepoMock = new Mock<IProductoRepository>();
            _categoriaRepoMock = new Mock<ICategoriaRepository>();
            _pdfServiceMock = new Mock<IPdfService>();
            _notificacionServiceMock = new Mock<INotificacionService>();

            _sut = new ReporteService(
                _productoRepoMock.Object,
                _categoriaRepoMock.Object,
                _pdfServiceMock.Object,
                _notificacionServiceMock.Object);
        }

        [Fact]
        public async Task GenerarReporteStockBajo_ConProductosBajoUmbral_RetornaProductosFiltrados()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 3, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Producto B", Stock = 10, Precio = 200, CategoriaId = 1 },
                new() { ProductoId = 3, Nombre = "Producto C", Stock = 2, Precio = 150, CategoriaId = 2 }
            };

            var categoria = new Categoria { CategoriaId = 1, Nombre = "Electrónicos" };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _sut.GenerarReporteStockBajo(umbral: 5);

            // Assert
            resultado.TotalProductos.Should().Be(2);
            resultado.Productos.Should().HaveCount(2);
            resultado.Productos.Should().OnlyContain(p => p.StockActual <= 5);
        }

        [Fact]
        public async Task GenerarReporteStockBajo_SinProductosBajoUmbral_RetornaListaVacia()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 10, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Producto B", Stock = 20, Precio = 200, CategoriaId = 1 }
            };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);

            // Act
            var resultado = await _sut.GenerarReporteStockBajo(umbral: 5);

            // Assert
            resultado.TotalProductos.Should().Be(0);
            resultado.Productos.Should().BeEmpty();
        }

        [Fact]
        public async Task GenerarReporteStockBajo_CalculaProductosCriticosCorrectamente()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Crítico", Stock = 2, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Bajo", Stock = 5, Precio = 200, CategoriaId = 1 },
                new() { ProductoId = 3, Nombre = "Crítico 2", Stock = 1, Precio = 150, CategoriaId = 1 }
            };

            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _sut.GenerarReporteStockBajo(umbral: 5);

            // Assert
            resultado.ProductosCriticos.Should().Be(2); // Stock < 5 son críticos
        }

        [Fact]
        public async Task GenerarReporteStockBajo_CalculaValorAfectadoCorrectamente()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 2, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Producto B", Stock = 3, Precio = 50, CategoriaId = 1 }
            };

            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _sut.GenerarReporteStockBajo(umbral: 5);

            // Assert
            // ValorAfectado = (2 * 100) + (3 * 50) = 200 + 150 = 350
            resultado.ValorAfectado.Should().Be(350);
        }

        [Fact]
        public async Task GenerarPdfStockBajo_LlamaServiciosEnOrdenCorrecto()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 2, Precio = 100, CategoriaId = 1 }
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };
            var htmlEsperado = "<html>Reporte</html>";
            var pdfEsperado = new byte[] { 1, 2, 3 };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);
            _pdfServiceMock.Setup(s => s.RenderizarTemplate(It.IsAny<string>(), It.IsAny<ReporteStockBajoResponse>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(htmlEsperado);
            _pdfServiceMock.Setup(s => s.GenerarPdfDesdeHtml(htmlEsperado, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pdfEsperado);

            // Act
            var resultado = await _sut.GenerarPdfStockBajo();

            // Assert
            resultado.Should().BeEquivalentTo(pdfEsperado);
            _pdfServiceMock.Verify(s => s.RenderizarTemplate("ReporteStockBajo", It.IsAny<ReporteStockBajoResponse>(), It.IsAny<CancellationToken>()), Times.Once);
            _pdfServiceMock.Verify(s => s.GenerarPdfDesdeHtml(htmlEsperado, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnviarReporteStockBajoPorEmail_EnviaPdfCorrectamente()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Producto A", Stock = 2, Precio = 100, CategoriaId = 1 }
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };
            var pdfBytes = new byte[] { 1, 2, 3 };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);
            _pdfServiceMock.Setup(s => s.RenderizarTemplate(It.IsAny<string>(), It.IsAny<ReporteStockBajoResponse>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("<html></html>");
            _pdfServiceMock.Setup(s => s.GenerarPdfDesdeHtml(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pdfBytes);

            // Act
            await _sut.EnviarReporteStockBajoPorEmail();

            // Assert
            _notificacionServiceMock.Verify(
                s => s.EnviarReporteStockBajoPorEmail(pdfBytes, It.IsAny<ReporteStockBajoResponse>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerarReporteStockBajo_OrdenaPorStockAscendente()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { ProductoId = 1, Nombre = "Stock 4", Stock = 4, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 2, Nombre = "Stock 1", Stock = 1, Precio = 100, CategoriaId = 1 },
                new() { ProductoId = 3, Nombre = "Stock 3", Stock = 3, Precio = 100, CategoriaId = 1 }
            };
            var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

            _productoRepoMock.Setup(r => r.GetAllActivas(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productos);
            _categoriaRepoMock.Setup(r => r.GetById(It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _sut.GenerarReporteStockBajo(umbral: 5);

            // Assert
            var listaProductos = resultado.Productos.ToList();
            listaProductos[0].StockActual.Should().Be(1);
            listaProductos[1].StockActual.Should().Be(3);
            listaProductos[2].StockActual.Should().Be(4);
        }
    }
}
