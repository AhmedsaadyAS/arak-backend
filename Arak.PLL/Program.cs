using Arak.BLL.Service.Abstraction;
using Arak.BLL.Service.Implementation;
using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Arak.DAL.Repository.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Arak.PLL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── 1. Controllers (with JSON cycle protection) ───────────────────
            builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    // Prevents infinite loops when EF navigation props are loaded.
                    // System.Text.Json will ignore already-serialised object references.
                    opts.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    opts.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            // ── 2. Database ───────────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── 3. ASP.NET Core Identity ──────────────────────────────────────
            // Provides UserManager, RoleManager, password hashing (bcrypt).
            // AddRoles<IdentityRole> enables role-based auth.
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Relax default rules to match current dev/test setup.
                    // Tighten in production via environment overrides.
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase        = true;
                    options.Password.RequireDigit            = true;
                    options.Password.RequiredLength          = 6;
                    options.User.RequireUniqueEmail          = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // ── 4. CORS ───────────────────────────────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    // Read origins from configuration for flexibility
                    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                  ?? new[] { "http://localhost:5173" };

                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // ── 5. JWT Authentication ─────────────────────────────────────────
            var jwtKey      = builder.Configuration["Jwt:Key"]!;
            var jwtIssuer   = builder.Configuration["Jwt:Issuer"]!;
            var jwtAudience = builder.Configuration["Jwt:Audience"]!;

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = jwtIssuer,
                        ValidAudience            = jwtAudience,
                        IssuerSigningKey         = new SymmetricSecurityKey(
                                                       Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // ── 6. Swagger with Bearer token support ──────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title       = "Arak API",
                    Version     = "v1",
                    Description = "ASP.NET Core 9 backend for the Arak School Admin Dashboard"
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Description  = "Enter: Bearer {your-jwt-token}",
                    In           = ParameterLocation.Header,
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "bearer",
                    BearerFormat = "JWT",
                    Reference    = new OpenApiReference
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

            // ── 7. Repositories ───────────────────────────────────────────────
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();

            // ── 8. Services ───────────────────────────────────────────────────
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITimetableService, TimetableService>();
            builder.Services.AddScoped<IAuthService, AuthService>();   // ← NEW
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IParentService, ParentService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IFeeService, FeeService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IEvaluationService, EvaluationService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();

            // ══════════════════════════════════════════════════════════════════
            var app = builder.Build();
            // ══════════════════════════════════════════════════════════════════

            // ── Step 4: Seed Database ─────────────────────────────────────────
            await Arak.DAL.Database.DbInitializer.InitializeAsync(app.Services);

            // ── Swagger (dev only) ────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ── Pipeline order (CRITICAL — do NOT reorder!) ───────────────────
            app.UseCors("AllowFrontend");       // 1. CORS — before auth
            if (!app.Environment.IsDevelopment())
                app.UseHttpsRedirection();       // 2. Force HTTPS (production only)
            app.UseExceptionHandler("/error");   // 3. Global error handler
            app.UseAuthentication();             // 4. Validate JWT
            app.UseAuthorization();              // 5. Enforce [Authorize]

            // ── Fallback error endpoint for exception handler ─────────────────
            app.Map("/error", (HttpContext context) =>
            {
                var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
                var statusCode = exception switch
                {
                    UnauthorizedAccessException => 401,
                    InvalidOperationException => 409,
                    KeyNotFoundException => 404,
                    _ => 500
                };

                // Provide specific exception message ONLY for 409 Conflict, otherwise use generic (except in Dev mode)
                var message = statusCode == 409 
                    ? exception?.Message 
                    : (app.Environment.IsDevelopment() ? exception?.Message : "An unexpected error occurred.");

                return Results.Problem(
                    title: statusCode == 409 ? "Data Conflict" : "An error occurred",
                    detail: message,
                    statusCode: statusCode);
            });

            app.MapControllers();

            app.Run();
        }
    }
}
