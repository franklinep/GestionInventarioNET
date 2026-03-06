using System;
using System.Collections.Generic;
using System.Text;

namespace GestionInventario.Application.Auth.DTOs
{
    public record LoginRequest(string Correo, string Password);
}
