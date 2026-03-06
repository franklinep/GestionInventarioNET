namespace GestionInventario.Domain.Entities
{
    public class Rol
    {
        public short RolId { get; set; }
        public string Nombre { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
