using GestionInventario.Application.Features.Auth.DTOs;
using GestionInventario.Application.Features.Auth.Interfaces;
using GestionInventario.Domain.Entities;

namespace GestionInventario.Application.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo = default!;
        private readonly IPasswordHasher _passHasher = default!;
        private readonly IJWTokenService _jwt = default!;

        public AuthService(IUsuarioRepository usuarioRepo, IPasswordHasher passHasher, IJWTokenService jwt)
        {
            _usuarioRepo = usuarioRepo;
            _passHasher = passHasher;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegistrarUsuario(RegisterRequest req, CancellationToken ct)
        {
            var correo = req.Correo.Trim();
            var usuario = await _usuarioRepo.GetByCorreo(correo, ct);
            if (usuario is not null)
                throw new InvalidOperationException("El correo ya se encuentra registrado.");

            var nuevoUsuario = new Usuario
            {
                Nombre = req.Nombre.Trim(),
                Correo = correo,
                PasswordHash = _passHasher.Hash(req.Password),
                IsActivo = true,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            var usuarioCreado = await _usuarioRepo.AddUsuario(nuevoUsuario, ct);

            var token = _jwt.GenerateToken(usuarioCreado);

            return new AuthResponse(
                token.token,
                token.expiresAtUtc,
                usuarioCreado.UsuarioId,
                usuarioCreado.Nombre,
                usuarioCreado.Correo
            );
        }

        public Task<AuthResponse> LoginUsuario(LoginRequest req, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

    }
}
