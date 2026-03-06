namespace GestionInventario.Domain.Entities
{
    public class Usuario
    {   
        public short UsuarioId { get; set; }
        public short RolId { get; set; } = 2; // empleado por defecto
        public string Nombre { get; set; } = default!;
        public string Correo { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsActivo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } 

        public Rol Rol { get; set; } = default!;
    }
}
