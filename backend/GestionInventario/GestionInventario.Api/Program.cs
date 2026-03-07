using GestionInventario.Application.Auth.Interfaces;
using GestionInventario.Application.Auth.Services;
using GestionInventario.Domain.Interfaces;
using GestionInventario.Infrastructure;
using GestionInventario.Infrastructure.Repositories;
using GestionInventario.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GestionInventario.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Se configura el DbContext 
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.Services.AddDbContext<InventarioDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("IventarioConn"))
            .UseSnakeCaseNamingConvention());

        // Se registran los repositorios
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Se registran los servicios 
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        builder.Services.AddScoped<IJWTokenService, JWTokenService>();

        // Se configura el JWT
        var jwtSecret = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret no configurado");
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestionInventario";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GestionInventarioApp";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

        builder.Services.AddAuthorization();

        // Se agrega los controladores
        builder.Services.AddControllers();

        // Se configura el swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
