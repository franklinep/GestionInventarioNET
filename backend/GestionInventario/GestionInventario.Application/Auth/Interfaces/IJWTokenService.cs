using GestionInventario.Domain.Entities;

namespace GestionInventario.Application.Auth.Interfaces
{
    public interface IJWTokenService
    {
        (string token, DateTime expiresAtUtc) GenerateToken(Usuario usuario);
    }
}
