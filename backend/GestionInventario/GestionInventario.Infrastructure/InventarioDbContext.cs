using Microsoft.EntityFrameworkCore;
using GestionInventario.Domain.Entities;

namespace GestionInventario.Infrastructure
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

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
            });

            mb.Entity<Categoria>(e =>
            {
                e.ToTable("categorias");
                e.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
                e.Property(x => x.IsActivo).HasDefaultValue(true);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            mb.Entity<Producto>(e =>
            {
                e.ToTable("productos");
                e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
                e.Property(x => x.Descripcion).HasColumnType("text");
                e.Property(x => x.Precio).HasPrecision(12, 2).IsRequired();
                e.Property(x => x.Stock).IsRequired();
                e.Property(x => x.IsActivo).HasDefaultValue(true);

                e.HasIndex(x => x.CategoriaId).HasDatabaseName("ix_productos_categoria");
                e.HasIndex(x => x.Stock).HasDatabaseName("ix_productos_stock");
                e.HasIndex(x => x.Nombre).HasDatabaseName("ix_productos_nombre");
            });
        }
    }
}
