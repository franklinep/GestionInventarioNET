using GestionInventario.Domain.Entities;

namespace GestionInventario.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByCorreo(string correo, CancellationToken ct);
        Task<Usuario> AddUsuario(Usuario usuario, CancellationToken ct);
    }
}
