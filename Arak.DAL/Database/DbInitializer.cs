using Arak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Arak.DAL.Database
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Roles
                var roles = new[] { "Super Admin", "Admin", "Academic Admin", "Teacher", "Fees Admin", "Users Admin", "Parent" };
                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // 2. Admin Users
                const string superAdminEmail = "superadmin@arak.com";
                var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
                if (superAdminUser == null)
                {
                    superAdminUser = new ApplicationUser
                    {
                        UserName = superAdminEmail, Email = superAdminEmail, EmailConfirmed = true,
                        Name = "Super Admin User", Address = "Arak School HQ"
                    };
                    var superAdminPassword = Environment.GetEnvironmentVariable("ARAK_SUPERADMIN_PASSWORD") ?? "SuperAdmin@123";
                    var createResult = await userManager.CreateAsync(superAdminUser, superAdminPassword);
                    if (createResult.Succeeded) await userManager.AddToRoleAsync(superAdminUser, "Super Admin");
                }

                const string adminEmail = "admin@arak.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail, Email = adminEmail, EmailConfirmed = true,
                        Name = "Admin User", Address = "Arak School HQ"
                    };
                    var adminPassword = Environment.GetEnvironmentVariable("ARAK_ADMIN_PASSWORD") ?? "Admin@123";
                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createResult.Succeeded) await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Ensure admin@arak.com has Admin role and NOT Super Admin role
                    if (await userManager.IsInRoleAsync(adminUser, "Super Admin"))
                    {
                        await userManager.RemoveFromRoleAsync(adminUser, "Super Admin");
                    }
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                const string academicEmail = "academic@arak.com";
                if (await userManager.FindByEmailAsync(academicEmail) == null)
                {
                    var academicUser = new ApplicationUser
                    {
                        UserName = academicEmail, Email = academicEmail, EmailConfirmed = true,
                        Name = "Academic Admin", Address = "Arak School HQ"
                    };
                    var academicPassword = Environment.GetEnvironmentVariable("ARAK_DEFAULT_PASSWORD") ?? "Academic@123";
                    var createResult = await userManager.CreateAsync(academicUser, academicPassword);
                    if (createResult.Succeeded) await userManager.AddToRoleAsync(academicUser, "Academic Admin");
                }

                // 3. Classes & Subjects
                if (!await dbContext.Classes.AnyAsync())
                {
                    dbContext.Classes.AddRange(
                        new Class { Name = "Grade 4-A",  Grade = "Grade 4",  Stage = "primary",      Description = "Primary - Grade 4A"      },
                        new Class { Name = "Grade 4-B",  Grade = "Grade 4",  Stage = "primary",      Description = "Primary - Grade 4B"      },
                        new Class { Name = "Grade 5-A",  Grade = "Grade 5",  Stage = "primary",      Description = "Primary - Grade 5A"      },
                        new Class { Name = "Grade 7-A",  Grade = "Grade 7",  Stage = "preparatory",  Description = "Preparatory - Grade 7A"  },
                        new Class { Name = "Grade 7-B",  Grade = "Grade 7",  Stage = "preparatory",  Description = "Preparatory - Grade 7B"  },
                        new Class { Name = "Grade 8-A",  Grade = "Grade 8",  Stage = "preparatory",  Description = "Preparatory - Grade 8A"  },
                        new Class { Name = "Grade 10-A", Grade = "Grade 10", Stage = "secondary",    Description = "Secondary - Grade 10A"   },
                        new Class { Name = "Grade 10-B", Grade = "Grade 10", Stage = "secondary",    Description = "Secondary - Grade 10B"   },
                        new Class { Name = "Grade 11-A", Grade = "Grade 11", Stage = "secondary",    Description = "Secondary - Grade 11A"   }
                    );
                    await dbContext.SaveChangesAsync();
                }

                if (!await dbContext.Subjects.AnyAsync())
                {
                    dbContext.Subjects.AddRange(
                        new Subject { Name = "Mathematics" },
                        new Subject { Name = "Physics"     },
                        new Subject { Name = "English"     },
                        new Subject { Name = "Arabic"      },
                        new Subject { Name = "Science"     }
                    );
                    await dbContext.SaveChangesAsync();
                }

                // 4. Teachers & Parents
                var parent1  = await CreateUser(userManager, roleManager, "parent1@arak.com",  "John Parent",   "Parent");
                var parent2  = await CreateUser(userManager, roleManager, "parent2@arak.com",  "Sarah Mother",  "Parent");
                var teacher1 = await CreateUser(userManager, roleManager, "teacher1@arak.com", "Ms. Maria",     "Teacher");
                var teacher2 = await CreateUser(userManager, roleManager, "teacher2@arak.com", "Mr. Anderson",  "Teacher");

                var math    = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "Mathematics");
                var english = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "English");

                if (!await dbContext.Teachers.AnyAsync(t => t.ApplicationUser.Email == "teacher1@arak.com"))
                {
                    dbContext.Teachers.Add(new Teacher { ApplicationUser = teacher1, SubjectId = math?.Id });
                    await dbContext.SaveChangesAsync();
                }
                if (!await dbContext.Teachers.AnyAsync(t => t.ApplicationUser.Email == "teacher2@arak.com"))
                {
                    dbContext.Teachers.Add(new Teacher { ApplicationUser = teacher2, SubjectId = english?.Id });
                    await dbContext.SaveChangesAsync();
                }

                if (!await dbContext.Parents.AnyAsync(p => p.ApplicationUser.Email == "parent1@arak.com"))
                {
                    dbContext.Parents.Add(new Parent { ApplicationUser = parent1 });
                    await dbContext.SaveChangesAsync();
                }
                if (!await dbContext.Parents.AnyAsync(p => p.ApplicationUser.Email == "parent2@arak.com"))
                {
                    dbContext.Parents.Add(new Parent { ApplicationUser = parent2 });
                    await dbContext.SaveChangesAsync();
                }

                // 5. Students
                var p1 = await dbContext.Parents.FirstOrDefaultAsync(p => p.ApplicationUser.Email == "parent1@arak.com");
                var c1 = await dbContext.Classes.FirstOrDefaultAsync(c => c.Name == "Grade 4-A");
                var c2 = await dbContext.Classes.FirstOrDefaultAsync(c => c.Name == "Grade 7-A");

                if (!await dbContext.Students.AnyAsync())
                {
                    dbContext.Students.AddRange(new List<Student>
                    {
                        new Student { Name = "Alice Parent",   StudentCode = "STU001", ClassId = c1?.Id, ParentId = p1?.ParentId, DateOfBirth = new DateTime(2015, 5,  20), Status = "Active",   Grade = "4" },
                        new Student { Name = "Bob Parent",     StudentCode = "STU002", ClassId = c2?.Id, ParentId = p1?.ParentId, DateOfBirth = new DateTime(2014, 8,  15), Status = "Active",   Grade = "7" },
                        new Student { Name = "Charlie Mother", StudentCode = "STU003", ClassId = c1?.Id,                         DateOfBirth = new DateTime(2015, 1,  10), Status = "Active",   Grade = "4" },
                        new Student { Name = "Diana Student",  StudentCode = "STU004", ClassId = c2?.Id,                         DateOfBirth = new DateTime(2014, 11, 30), Status = "Inactive", Grade = "7" },
                        new Student { Name = "Evan Scholar",   StudentCode = "STU005", ClassId = c1?.Id,                         DateOfBirth = new DateTime(2015, 4,  12), Status = "Active",   Grade = "4" }
                    });
                    await dbContext.SaveChangesAsync();
                }

                // 6. TimeTables ✅ Location + TeacherId مضاف
                if (!await dbContext.TimeTables.AnyAsync())
                {
                    var mathSub = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "Mathematics");
                    var physSub = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "Physics");
                    var engSub  = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "English");
                    var arabSub = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "Arabic");
                    var sciSub  = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "Science");

                    var grade4a = await dbContext.Classes.FirstOrDefaultAsync(c => c.Name == "Grade 4-A");
                    var grade7a = await dbContext.Classes.FirstOrDefaultAsync(c => c.Name == "Grade 7-A");

                    // Resolve teacher domain IDs for foreign key
                    var t1 = await dbContext.Teachers.Include(t => t.ApplicationUser)
                                .FirstOrDefaultAsync(t => t.ApplicationUser.Email == "teacher1@arak.com");
                    var t2 = await dbContext.Teachers.Include(t => t.ApplicationUser)
                                .FirstOrDefaultAsync(t => t.ApplicationUser.Email == "teacher2@arak.com");

                    dbContext.TimeTables.AddRange(
                        new TimeTable { ClassId = grade4a?.Id, SubjectId = mathSub?.Id, TeacherId = t1?.TeacherId, DayOfWeek = DayOfWeek.Sunday,  StartTime = TimeSpan.FromHours(8),  EndTime = TimeSpan.FromHours(9.5),  Location = "Room 101" },
                        new TimeTable { ClassId = grade4a?.Id, SubjectId = engSub?.Id,  TeacherId = t2?.TeacherId, DayOfWeek = DayOfWeek.Sunday,  StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11.5), Location = "Room 102" },
                        new TimeTable { ClassId = grade4a?.Id, SubjectId = arabSub?.Id, TeacherId = t1?.TeacherId, DayOfWeek = DayOfWeek.Monday,  StartTime = TimeSpan.FromHours(8),  EndTime = TimeSpan.FromHours(9.5),  Location = "Room 101" },
                        new TimeTable { ClassId = grade4a?.Id, SubjectId = sciSub?.Id,  TeacherId = t2?.TeacherId, DayOfWeek = DayOfWeek.Monday,  StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11.5), Location = "Room 102" },
                        new TimeTable { ClassId = grade7a?.Id, SubjectId = mathSub?.Id, TeacherId = t1?.TeacherId, DayOfWeek = DayOfWeek.Sunday,  StartTime = TimeSpan.FromHours(8),  EndTime = TimeSpan.FromHours(9.5),  Location = "Room 201" },
                        new TimeTable { ClassId = grade7a?.Id, SubjectId = physSub?.Id, TeacherId = t2?.TeacherId, DayOfWeek = DayOfWeek.Monday,  StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11.5), Location = "Lab 1"    },
                        new TimeTable { ClassId = grade7a?.Id, SubjectId = engSub?.Id,  TeacherId = t2?.TeacherId, DayOfWeek = DayOfWeek.Tuesday, StartTime = TimeSpan.FromHours(8),  EndTime = TimeSpan.FromHours(9.5),  Location = "Room 202" }
                    );
                    await dbContext.SaveChangesAsync();
                }

                // 7. Simulated Conversation Messages
                if (true) // Resets and seeds a clean simulated history for developers
                {
                    // Clear out old messages first to avoid duplicates or skipping
                    var existing = await dbContext.Messages.ToListAsync();
                    if (existing.Any())
                    {
                        dbContext.Messages.RemoveRange(existing);
                        await dbContext.SaveChangesAsync();
                    }

                    var admin = await userManager.FindByEmailAsync("admin@arak.com");
                    var superadmin = await userManager.FindByEmailAsync("superadmin@arak.com");
                    var t1User = await userManager.FindByEmailAsync("teacher1@arak.com");
                    var t2User = await userManager.FindByEmailAsync("teacher2@arak.com");
                    var p1User = await userManager.FindByEmailAsync("parent1@arak.com");
                    var p2User = await userManager.FindByEmailAsync("parent2@arak.com");

                    if (admin != null && superadmin != null && t1User != null && t2User != null && p1User != null && p2User != null)
                    {
                        var now = DateTime.UtcNow;
                        dbContext.Messages.AddRange(
                            // Incoming messages to Admin (counted as Received Messages on Admin Dashboard)
                            new Message { SenderId = t1User.Id, ReceiverId = admin.Id, Content = "Hello Admin, I have successfully updated the math exam grades for Grade 4-A.", SentAt = now.AddHours(-5) },
                            new Message { SenderId = t2User.Id, ReceiverId = admin.Id, Content = "Hi, could we double check the timetable classroom assignments for next Monday?", SentAt = now.AddHours(-4) },
                            new Message { SenderId = p1User.Id, ReceiverId = admin.Id, Content = "Hello, is there any updates on Alice's school bus registration?", SentAt = now.AddHours(-3) },
                            new Message { SenderId = p2User.Id, ReceiverId = admin.Id, Content = "Good afternoon, I wanted to submit Charlie's medical report for physical education exemption.", SentAt = now.AddHours(-2) },
                            new Message { SenderId = t1User.Id, ReceiverId = admin.Id, Content = "Admin, please approve the school trip request for Grade 4-A by tomorrow.", SentAt = now.AddHours(-1) },

                            // Incoming messages to Super Admin (counted as Received Messages on Super Admin Dashboard)
                            new Message { SenderId = t1User.Id, ReceiverId = superadmin.Id, Content = "Dear Super Admin, I wanted to report that the Grade 4-A classroom projector requires maintenance.", SentAt = now.AddHours(-3) },
                            new Message { SenderId = p1User.Id, ReceiverId = superadmin.Id, Content = "Hello, could we schedule a brief meeting regarding the upcoming parent-teacher council nominations?", SentAt = now.AddHours(-2) },
                            new Message { SenderId = t2User.Id, ReceiverId = superadmin.Id, Content = "Good afternoon, the new English curriculum materials have been distributed successfully to Grade 7.", SentAt = now.AddHours(-1) },

                            // Simulated Teacher-Parent Conversations (Mobile App API alignment)
                            new Message { SenderId = t1User.Id, ReceiverId = p1User.Id, Content = "Dear Mr. John, Alice participated actively in class today!", SentAt = now.AddDays(-1).AddHours(1) },
                            new Message { SenderId = p1User.Id, ReceiverId = t1User.Id, Content = "Thank you Ms. Maria, she really loves your math class!", SentAt = now.AddDays(-1).AddHours(2) }
                        );
                        await dbContext.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, string email, string name, string role)
        {
            var user = await um.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, Name = name, EmailConfirmed = true };
                var defaultPassword = Environment.GetEnvironmentVariable("ARAK_DEFAULT_PASSWORD") ?? $"{role}@123";
                var res = await um.CreateAsync(user, defaultPassword);
                if (res.Succeeded)
                {
                    await um.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new InvalidOperationException($"Failed to create user {email}: {string.Join(", ", res.Errors.Select(e => e.Description))}");
                }
            }
            return user;
        }
    }
}