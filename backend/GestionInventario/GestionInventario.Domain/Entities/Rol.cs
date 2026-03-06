using System;
using System.Collections.Generic;
using System.Text;

namespace GestionInventario.Domain.Entities
{
    public class Rol
    {
        public short RolId { get; set; }
        public string Nombre { get; set; } = "admin";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
