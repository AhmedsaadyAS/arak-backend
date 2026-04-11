using Arak.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Arak.DAL.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global rule: prevent accidental cascade deletes across the entire model.
            // Applied to ALL relationships including the new ArakEvent, Fee, Evaluation FKs.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Store the DayOfWeek enum as a string in the database for readability.
            modelBuilder.Entity<TimeTable>()
                .Property(e => e.DayOfWeek)
                .HasConversion<string>();

            // Seed data — Gender lookup table
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = 1, Name = "Male" },
                new Gender { Id = 2, Name = "Female" }
            );
        }

        // ── Identity ──────────────────────────────────────────────────────────
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // ── Core People ───────────────────────────────────────────────────────
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        // ── Academic Structure ────────────────────────────────────────────────
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<GradeLevel> GradeLevels { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Gender> Genders { get; set; }

        // ── Relationships (Junction Tables) ───────────────────────────────────
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<TeacherClass> TeacherClasses { get; set; }
        public DbSet<TeacherSemester> TeacherSemesters { get; set; }

        // ── Scheduling & Attendance ───────────────────────────────────────────
        public DbSet<TimeTable> TimeTables { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        // ── New Entities (Step 2) ─────────────────────────────────────────────
        public DbSet<ArakEvent> Events { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
    }
}
