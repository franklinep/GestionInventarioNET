using System;
using System.Collections.Generic;
using System.Text;

namespace GestionInventario.Application.Features.Auth.DTOs
{
    public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, short UsuarioId, string Nombre, string Correo);
}
