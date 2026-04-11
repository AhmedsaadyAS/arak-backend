using Arak.BLL.Service.Abstraction;
using Arak.BLL.Service.Implementation;
using Arak.DAL.Database;
using Arak.DAL.Repository.Abstraction;
using Arak.DAL.Repository.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Arak.PLL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── 1. Controllers ────────────────────────────────────────────────
            builder.Services.AddControllers();

            // ── 2. Database ───────────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── 3. CORS ───────────────────────────────────────────────────────
            // Allows the Arak Admin Dashboard (Vite: 5173, Dashboard: 3000)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",
                            "http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ── 4. JWT Authentication ─────────────────────────────────────────
            var jwtKey    = builder.Configuration["Jwt:Key"]!;
            var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
            var jwtAudience = builder.Configuration["Jwt:Audience"]!;

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey  = true,
                        ValidIssuer              = jwtIssuer,
                        ValidAudience            = jwtAudience,
                        IssuerSigningKey         = new SymmetricSecurityKey(
                                                       Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // ── 5. Swagger with Bearer Token Support ──────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title   = "Arak API",
                    Version = "v1",
                    Description = "ASP.NET Core 9 backend for the Arak School Admin Dashboard"
                });

                // Allow Swagger UI to send Bearer tokens
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Description  = "Enter 'Bearer {token}'",
                    In           = ParameterLocation.Header,
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

            // ── 6. Repositories ───────────────────────────────────────────────
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();

            // ── 7. Services ────────────────────────────────────────────────────
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITimetableService, TimetableService>();

            // ══════════════════════════════════════════════════════════════════
            var app = builder.Build();
            // ══════════════════════════════════════════════════════════════════

            // ── Swagger (dev only) ────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ── Pipeline order (CRITICAL — do NOT reorder!) ───────────────────
            // 1. CORS must be first so preflight OPTIONS requests are handled
            app.UseCors("AllowFrontend");

            // 2. Authentication must come before Authorization
            app.UseAuthentication();

            // 3. Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
