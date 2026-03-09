namespace GestionInventario.Domain.Entities
{
    public class Producto
    {
        public short ProductoId { get; set; }
        public short CategoriaId { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool IsActivo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public short CreatedBy { get; set; }
        public short UpdatedBy { get; set; }
    }
}
