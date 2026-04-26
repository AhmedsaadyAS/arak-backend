using Arak.BLL.DTOs;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ARAK.PLL.Controllers
{
    /// <summary>
    /// Teacher CRUD — merges Teacher entity with ApplicationUser profile data.
    /// The Teacher entity only has FKs; name/email/phone live on ApplicationUser.
    /// We use UserManager to load the profile, then project to TeacherDto.
    /// PATCH /api/teachers/{id} handles partial updates including assignedClasses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Generic authorize to ensure user is logged in, role specific rules are on methods
    public class TeachersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Arak.DAL.Database.AppDbContext _db;

        public TeachersController(
            UserManager<ApplicationUser> userManager,
            Arak.DAL.Database.AppDbContext db)
        {
            _userManager = userManager;
            _db          = db;
        }

        // ── GET /api/teachers ────────────────────────────────────────────────
        // Optional ?q=name search
        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] string? q = null)
        {
            var query = _db.Teachers
                .Include(t => t.ApplicationUser)
                .Include(t => t.Subject)
                .Include(t => t.Classes)
                    .ThenInclude(tc => tc.Class)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var lower = q.ToLower();
                query = query.Where(t =>
                    t.ApplicationUser != null &&
                    (t.ApplicationUser.Name!.ToLower().Contains(lower) ||
                     t.ApplicationUser.Email!.ToLower().Contains(lower)));
            }

            var teachers = await query.ToListAsync();
            var dtos = teachers.Select((t, i) => MapToDto(t, i + 1));
            return Ok(dtos);
        }

        // ── GET /api/teachers/{id} ───────────────────────────────────────────
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var teacher = await _db.Teachers
                .Include(t => t.ApplicationUser)
                .Include(t => t.Subject)
                .Include(t => t.Classes)
                    .ThenInclude(tc => tc.Class)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null)
                return NotFound(new { message = $"Teacher {id} not found." });

            return Ok(MapToDto(teacher, id));
        }

        // ── GET /api/teachers/me ────────────────────────────────────────────
        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User claim not found." });

            var teacher = await _db.Teachers
                .Include(t => t.ApplicationUser)
                .Include(t => t.Subject)
                .Include(t => t.Classes)
                    .ThenInclude(tc => tc.Class)
                .FirstOrDefaultAsync(t => t.ApplicationUser != null && t.ApplicationUser.Id == userId);

            if (teacher == null)
                return NotFound(new { message = "No teacher profile linked to this account." });

            return Ok(new
            {
                teacherId       = teacher.TeacherId,
                name            = teacher.ApplicationUser?.Name ?? "",
                email           = teacher.ApplicationUser?.Email ?? "",
                phone           = teacher.ApplicationUser?.PhoneNumber,
                subject         = teacher.Subject?.Name ?? "",
                subjectId       = teacher.SubjectId,
                assignedClasses = teacher.Classes?.Select(tc => new
                {
                    classId   = tc.ClassId,
                    className = tc.Class?.Name ?? ""
                }).ToList(),
                todayClassesCount = 0,
                hasNewTasks       = false,
                performance       = 0.0
            });
        }

        // ── POST /api/teachers ───────────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> CreateAsync([FromBody] TeacherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { errors = new[] { "Email is required." } });

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            ApplicationUser user;

            if (existingUser != null)
            {
                // User exists — check if teacher already linked
                var existingTeacher = await _db.Teachers
                    .FirstOrDefaultAsync(t => t.ApplicationUser != null && t.ApplicationUser.Id == existingUser.Id);
                if (existingTeacher != null)
                    return BadRequest(new { errors = new[] { "A teacher with this email already exists." } });

                user = existingUser;
            }
            else
            {
                user = new ApplicationUser
                {
                    UserName       = dto.Email,
                    Email          = dto.Email,
                    Name           = dto.Name,
                    PhoneNumber    = dto.PhoneNumber,
                    Address        = dto.Address,
                    EmailConfirmed = true
                };

                var defaultPassword = Environment.GetEnvironmentVariable("ARAK_DEFAULT_PASSWORD") ?? "Teacher@123";
                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            // Ensure user has Teacher role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Teacher"))
                await _userManager.AddToRoleAsync(user, "Teacher");

            int? subjectId = dto.SubjectId;
            if (subjectId == null && !string.IsNullOrEmpty(dto.Subject))
            {
                var s = await _db.Subjects.FirstOrDefaultAsync(x => x.Name == dto.Subject);
                subjectId = s?.Id;
            }

            var teacher = new Teacher
            {
                ApplicationUser = user,
                SubjectId       = subjectId
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            return Ok(MapToDto(teacher, teacher.TeacherId));
        }

        // ── PUT /api/teachers/{id} ───────────────────────────────────────────
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] TeacherDto dto)
        {
            var teacher = await _db.Teachers
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null) return NotFound();

            if (teacher.ApplicationUser != null)
            {
                if (!string.IsNullOrEmpty(dto.Name))
                    teacher.ApplicationUser.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    teacher.ApplicationUser.Email    = dto.Email;
                    teacher.ApplicationUser.UserName = dto.Email;
                }
                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    teacher.ApplicationUser.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrEmpty(dto.Address))
                    teacher.ApplicationUser.Address = dto.Address;
                await _userManager.UpdateAsync(teacher.ApplicationUser);
            }

            if (dto.SubjectId.HasValue)
            {
                teacher.SubjectId = dto.SubjectId;
            }
            else if (!string.IsNullOrEmpty(dto.Subject))
            {
                var s = await _db.Subjects.FirstOrDefaultAsync(x => x.Name == dto.Subject);
                if (s != null) teacher.SubjectId = s.Id;
            }

            await _db.SaveChangesAsync();
            return Ok(MapToDto(teacher, id));
        }

        // ── PATCH /api/teachers/{id} ─────────────────────────────────────────
        // Handles partial updates — primarily used by scheduleService.js
        // to keep teacher.assignedClasses in sync when schedules are added/removed.
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] PatchTeacherDto dto)
        {
            var teacher = await _db.Teachers
                .Include(t => t.ApplicationUser)
                .Include(t => t.Classes)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null) return NotFound(new { message = $"Teacher {id} not found." });

            // Update basic profile fields if provided
            if (teacher.ApplicationUser != null)
            {
                if (dto.Name        is not null) teacher.ApplicationUser.Name        = dto.Name;
                if (dto.PhoneNumber is not null) teacher.ApplicationUser.PhoneNumber = dto.PhoneNumber;
                if (dto.Address     is not null) teacher.ApplicationUser.Address     = dto.Address;
                if (dto.Name is not null || dto.PhoneNumber is not null || dto.Address is not null)
                    await _userManager.UpdateAsync(teacher.ApplicationUser);
            }

            // Sync assignedClasses — replace the TeacherClass junction rows
            if (dto.AssignedClasses is not null)
            {
                // Remove all existing class assignments
                var existing = _db.TeacherClasses.Where(tc => tc.TeacherId == id);
                _db.TeacherClasses.RemoveRange(existing);

                // Add the new ones
                foreach (var classId in dto.AssignedClasses.Distinct())
                {
                    _db.TeacherClasses.Add(new TeacherClass
                    {
                        TeacherId = id,
                        ClassId   = classId
                    });
                }
            }

            if (dto.SubjectId.HasValue)
                teacher.SubjectId = dto.SubjectId;

            await _db.SaveChangesAsync();

            // Reload with navigation to build a fresh DTO
            await _db.Entry(teacher).Collection(t => t.Classes).LoadAsync();
            return Ok(MapToDto(teacher, id));
        }

        // ── DELETE /api/teachers/{id} ────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var teacher = await _db.Teachers
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.TeacherId == id);
                
            if (teacher == null) return NotFound();

            // 1. Remove Teacher domain row
            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();

            // 2. Remove Teacher role and potentially the user
            if (teacher.ApplicationUser != null)
            {
                if (await _userManager.IsInRoleAsync(teacher.ApplicationUser, "Teacher"))
                {
                    var uid = teacher.ApplicationUser.Id;
                    await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserRoles WHERE UserId = '{uid}' AND RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Teacher')");
                }

                var remainingRoles = await _userManager.GetRolesAsync(teacher.ApplicationUser);
                if (!remainingRoles.Any())
                {
                    var uid = teacher.ApplicationUser.Id;
                    await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserRoles WHERE UserId = '{uid}'");
                    await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserClaims WHERE UserId = '{uid}'");
                    await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserLogins WHERE UserId = '{uid}'");
                    await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserTokens WHERE UserId = '{uid}'");

                    await _userManager.DeleteAsync(teacher.ApplicationUser);
                }
            }

            return Ok(new { message = "Teacher deleted." });
        }

        // ── DTO mapping helper ─────────────────────────────────────────────

        private TeacherDto MapToDto(Teacher t, int displayIndex)
        {
            var user = t.ApplicationUser;
            return new TeacherDto
            {
                Id             = t.TeacherId,
                // Deterministic format based on the actual TeacherId (not array index)
                TeacherId      = $"#T{100000 + t.TeacherId}",
                Name           = user?.Name        ?? string.Empty,
                Email          = user?.Email       ?? string.Empty,
                PhoneNumber    = user?.PhoneNumber ?? null,
                Address        = user?.Address     ?? null,
                Subject        = t.Subject?.Name   ?? null,
                SubjectId      = t.SubjectId,
                Department     = null,
                Experience     = null,
                City           = null,
                // Resolve assignedClasses from the TeacherClass junction table
                AssignedClasses = t.Classes?.Select(tc => tc.ClassId).ToArray() ?? [],
            };
        }
    }
}
