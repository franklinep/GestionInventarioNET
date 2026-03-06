using GestionInventario.Application.Features.Auth.DTOs;

namespace GestionInventario.Application.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegistrarUsuario(RegisterRequest req, CancellationToken ct);
        Task<AuthResponse> LoginUsuario(LoginRequest req, CancellationToken ct);
    }
}
