using GestionInventario.Application.Auth.Interfaces;
using GestionInventario.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionInventario.Infrastructure.Services
{
    public class JWTokenService : IJWTokenService
    {
        private readonly IConfiguration _configuration;

        public JWTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string token, DateTime expiresAtUtc) GenerateToken(Usuario usuario)
        {
            var secret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret no configurado");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Mapeo simple de RolId a nombre (1 = admin, 2 = empleado)
            var rolNombre = usuario.RolId == 1 ? "admin" : "empleado";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, rolNombre),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}
