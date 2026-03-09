namespace GestionInventario.Domain.Entities
{
    public class Categoria
    {
        public short CategoriaId { get; set; }
        public string Nombre { get; set; } = default!;
        public bool IsActivo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
