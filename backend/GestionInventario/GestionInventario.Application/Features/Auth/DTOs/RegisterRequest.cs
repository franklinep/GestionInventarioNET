using System;
using System.Collections.Generic;
using System.Text;

namespace GestionInventario.Application.Features.Auth.DTOs
{
    public record RegisterRequest(string Nombre, string Correo, string Password);
}
