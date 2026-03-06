using GestionInventario.Application.Auth.DTOs;

namespace GestionInventario.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegistrarUsuario(RegisterRequest req, CancellationToken ct);
        Task<AuthResponse> LoginUsuario(LoginRequest req, CancellationToken ct);
    }
}
