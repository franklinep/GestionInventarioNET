using FluentAssertions;
using GestionInventario.Application.Auth.DTOs;
using GestionInventario.Application.Auth.Interfaces;
using GestionInventario.Application.Auth.Services;
using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestionInventario.Test.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJWTokenService> _jwtServiceMock;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtServiceMock = new Mock<IJWTokenService>();

            _sut = new AuthService(
                _usuarioRepoMock.Object,
                _passwordHasherMock.Object,
                _jwtServiceMock.Object);
        }

        [Fact]
        public async Task RegistrarUsuario_CorreoNuevo_CreaUsuarioYRetornaToken()
        {
            // Arrange
            var request = new RegisterRequest("Juan Pérez", "juan@test.com", "Password123");
            var usuarioCreado = new Usuario
            {
                UsuarioId = 1,
                Nombre = "Juan Pérez",
                Correo = "juan@test.com",
                PasswordHash = "hashedPassword",
                IsActivo = true
            };

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);
            _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>()))
                .Returns("hashedPassword");
            _usuarioRepoMock.Setup(r => r.AddUsuario(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioCreado);
            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<Usuario>()))
                .Returns(("token123", DateTime.UtcNow.AddHours(1)));

            // Act
            var resultado = await _sut.RegistrarUsuario(request, CancellationToken.None);

            // Assert
            resultado.AccessToken.Should().Be("token123");
            resultado.UsuarioId.Should().Be(1);
            resultado.Nombre.Should().Be("Juan Pérez");
        }

        [Fact]
        public async Task RegistrarUsuario_CorreoExistente_LanzaExcepcion()
        {
            // Arrange
            var request = new RegisterRequest("Juan Pérez", "existente@test.com", "Password123");
            var usuarioExistente = new Usuario { Correo = "existente@test.com" };

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act
            var act = () => _sut.RegistrarUsuario(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El correo ya se encuentra registrado.");
        }

        [Fact]
        public async Task LoginUsuario_CredencialesCorrectas_RetornaToken()
        {
            // Arrange
            var request = new LoginRequest("juan@test.com", "Password123");
            var usuario = new Usuario
            {
                UsuarioId = 1,
                Nombre = "Juan Pérez",
                Correo = "juan@test.com",
                PasswordHash = "hashedPassword",
                IsActivo = true
            };

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<Usuario>()))
                .Returns(("token123", DateTime.UtcNow.AddHours(1)));

            // Act
            var resultado = await _sut.LoginUsuario(request, CancellationToken.None);

            // Assert
            resultado.AccessToken.Should().Be("token123");
            resultado.UsuarioId.Should().Be(1);
        }

        [Fact]
        public async Task LoginUsuario_CorreoNoRegistrado_LanzaExcepcion()
        {
            // Arrange
            var request = new LoginRequest("noexiste@test.com", "Password123");

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var act = () => _sut.LoginUsuario(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Correo no registrado");
        }

        [Fact]
        public async Task LoginUsuario_UsuarioInactivo_LanzaExcepcion()
        {
            // Arrange
            var request = new LoginRequest("juan@test.com", "Password123");
            var usuario = new Usuario
            {
                UsuarioId = 1,
                Correo = "juan@test.com",
                IsActivo = false
            };

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var act = () => _sut.LoginUsuario(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario inactivo, por favor comunicarse con el administrador");
        }

        [Fact]
        public async Task LoginUsuario_PasswordIncorrecto_LanzaExcepcion()
        {
            // Arrange
            var request = new LoginRequest("juan@test.com", "WrongPassword");
            var usuario = new Usuario
            {
                UsuarioId = 1,
                Correo = "juan@test.com",
                PasswordHash = "hashedPassword",
                IsActivo = true
            };

            _usuarioRepoMock.Setup(r => r.GetByCorreo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            // Act
            var act = () => _sut.LoginUsuario(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Contraseña incorrecta");
        }
    }
}
