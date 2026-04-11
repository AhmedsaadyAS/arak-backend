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

            // ── 1. Controllers ────────────────────────────────────────────────
            builder.Services.AddControllers();

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
                    policy.WithOrigins(
                            "http://localhost:5173",  // Vite dev server
                            "http://localhost:3000")  // Arak Admin Dashboard
                          .AllowAnyHeader()
                          .AllowAnyMethod();
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

            // ══════════════════════════════════════════════════════════════════
            var app = builder.Build();
            // ══════════════════════════════════════════════════════════════════

            // ── Step 4: Seed test admin user on startup ───────────────────────
            await SeedAdminUserAsync(app);

            // ── Swagger (dev only) ────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ── Pipeline order (CRITICAL — do NOT reorder!) ───────────────────
            app.UseCors("AllowFrontend");       // 1. CORS — before auth
            app.UseAuthentication();             // 2. Validate JWT
            app.UseAuthorization();              // 3. Enforce [Authorize]

            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// Seeds a default admin user and the standard role set on first run.
        /// Roles match the exact strings the frontend RBAC expects (BACKEND.md §1 + §13).
        /// </summary>
        private static async Task SeedAdminUserAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ── Seed all roles the frontend RBAC checks against ───────────────
            // Do NOT rename these — the frontend does exact string comparison.
            var roles = new[]
            {
                "Super Admin",
                "Admin",
                "Academic Admin",
                "Teacher",
                "Fees Admin",
                "Users Admin",
                "Parent"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // ── Seed default admin user ───────────────────────────────────────
            const string adminEmail    = "admin@arak.com";
            const string adminPassword = "Admin@123";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin is null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName       = adminEmail,
                    Email          = adminEmail,
                    EmailConfirmed = true,
                    Name           = "Admin User",
                    Address        = "Arak School HQ"
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Log errors to console for DevOps visibility
                    foreach (var error in createResult.Errors)
                        Console.WriteLine($"[Seed Error] {error.Code}: {error.Description}");
                }
            }
        }
    }
}
