using System;
using System.Collections.Generic;
using System.Text;

namespace GestionInventario.Application.Features.Auth.DTOs
{
    public record LoginRequest(string Correo, string Password);
}
