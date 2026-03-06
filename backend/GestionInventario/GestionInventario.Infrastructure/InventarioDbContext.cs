using Microsoft.EntityFrameworkCore;
using GestionInventario.Domain.Entities;

namespace GestionInventario.Infrastructure
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.HasPostgresExtension("citext");

            mb.Entity<Rol>(e =>
            {
                e.ToTable("roles");
                e.Property(x => x.Nombre).HasMaxLength(20).IsRequired();
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            mb.Entity<Usuario>(e =>
            {
                e.ToTable("usuarios");
                e.Property(x => x.Nombre).HasMaxLength(50).IsRequired();
                e.Property(x => x.Correo).HasColumnType("citext").HasMaxLength(100).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                e.HasIndex(x => x.Correo).IsUnique();

                e.HasOne(x => x.Rol)
                    .WithMany()
                    .HasForeignKey(x => x.RolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
