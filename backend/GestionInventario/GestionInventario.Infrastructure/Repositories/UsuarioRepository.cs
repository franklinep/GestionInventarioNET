using GestionInventario.Domain.Entities;
using GestionInventario.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionInventario.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly InventarioDbContext _db;

        public UsuarioRepository(InventarioDbContext db)
        {
            _db = db;
        }

        public async Task<Usuario?> GetByCorreo(string correo, CancellationToken ct)
        {
            return await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo, ct);
        }

        public async Task<Usuario> AddUsuario(Usuario usuario, CancellationToken ct)
        {
            await _db.Usuarios.AddAsync(usuario, ct);
            await _db.SaveChangesAsync(ct);
            return usuario;
        }
    }
}
