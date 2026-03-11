using GestionInventario.Application.Auth.Interfaces;
using GestionInventario.Application.Auth.Services;
using GestionInventario.Application.Categorias.Interfaces;
using GestionInventario.Application.Categorias.Services;
using GestionInventario.Application.Dashboard.Interfaces;
using GestionInventario.Application.Dashboard.Services;
using GestionInventario.Application.Productos.Interfaces;
using GestionInventario.Application.Productos.Services;
using GestionInventario.Application.Reportes.Interfaces;
using GestionInventario.Application.Reportes.Services;
using GestionInventario.Domain.Interfaces;
using GestionInventario.Infrastructure;
using GestionInventario.Infrastructure.Repositories;
using GestionInventario.Infrastructure.Services;
using GestionInventario.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace GestionInventario.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.Services.AddDbContext<InventarioDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("IventarioConn"))
            .UseSnakeCaseNamingConvention());

        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        builder.Services.AddScoped<IJWTokenService, JWTokenService>();
        builder.Services.AddScoped<ICategoriaService, CategoriaService>();
        builder.Services.AddScoped<IProductoService, ProductoService>();
        builder.Services.AddScoped<IImagenService, ImagenService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();

        builder.Services.AddScoped<IReporteService, ReporteService>();
        builder.Services.AddScoped<INotificacionService, EmailNotificationService>();

        builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        builder.Services.AddScoped<IPdfService, PdfService>();

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

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));
        });

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseExceptionHandling();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
