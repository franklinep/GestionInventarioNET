using GestionInventario.Application.Features.Auth.DTOs;

namespace GestionInventario.Application.Features.Auth.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string plainText);
        bool Verify(string plainText, string hash);
    }
}
