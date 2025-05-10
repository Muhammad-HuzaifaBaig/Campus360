using Campus360.Data.Enum;
using Campus360.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Campus360.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<User> userManager,
                                         RoleManager<IdentityRole> roleManager,
                                         AppDbContext context)
        {
            // 1. Ensure database created and migrated
            await context.Database.MigrateAsync();

            // 2. Create roles
            var roles = new[] { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 3. Create Admin User
            User adminUser = null;
            if (await userManager.FindByEmailAsync("admin@campus360.com") == null)
            {
                adminUser = new User
                {
                    UserName = "admin@campus360.com",
                    Email = "admin@campus360.com",
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 4. Create Teachers
            User teacher1 = null, teacher2 = null;
            if (await userManager.FindByEmailAsync("john.doe@campus360.com") == null)
            {
                teacher1 = new User
                {
                    UserName = "john.doe@campus360.com",
                    Email = "john.doe@campus360.com",
                    FullName = "John Doe",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher1, "Teacher@123");
                await userManager.AddToRoleAsync(teacher1, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher1.Id });
            }

            if (await userManager.FindByEmailAsync("jane.smith@campus360.com") == null)
            {
                teacher2 = new User
                {
                    UserName = "jane.smith@campus360.com",
                    Email = "jane.smith@campus360.com",
                    FullName = "Jane Smith",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher2, "Teacher@123");
                await userManager.AddToRoleAsync(teacher2, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher2.Id });
            }

            // 5. Create Classes if none exist
            if (!context.Classes.Any())
            {
                context.Classes.AddRange(
                    new Class { Capacity = 30 },
                    new Class { Capacity = 25 },
                    new Class { Capacity = 35 }
                );
                await context.SaveChangesAsync();
            }

            // 6. Create Students
            User student1 = null, student2 = null;
            if (await userManager.FindByEmailAsync("alice.johnson@campus360.com") == null)
            {
                student1 = new User
                {
                    UserName = "alice.johnson@campus360.com",
                    Email = "alice.johnson@campus360.com",
                    FullName = "Alice Johnson",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student1, "Student@123");
                await userManager.AddToRoleAsync(student1, "Student");

                context.Students.Add(new Student
                {
                    UserId = student1.Id,
                    ClassId = 1,
                    Semester = Semester.First
                });
            }

            if (await userManager.FindByEmailAsync("bob.williams@campus360.com") == null)
            {
                student2 = new User
                {
                    UserName = "bob.williams@campus360.com",
                    Email = "bob.williams@campus360.com",
                    FullName = "Bob Williams",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student2, "Student@123");
                await userManager.AddToRoleAsync(student2, "Student");

                context.Students.Add(new Student
                {
                    UserId = student2.Id,
                    ClassId = 1,
                    Semester = Semester.First
                });
            }

            // 7. Create Courses if none exist
            if (!context.Courses.Any())
            {
                context.Courses.AddRange(
                    new Course { Name = "Mathematics", CreditHours = CreditHours.Three, Semester = Semester.First },
                    new Course { Name = "Physics", CreditHours = CreditHours.Four, Semester = Semester.First },
                    new Course { Name = "Computer Science", CreditHours = CreditHours.Three, Semester = Semester.Second },
                    new Course { Name = "English Literature", CreditHours = CreditHours.Two, Semester = Semester.Second }
                );
                await context.SaveChangesAsync();
            }

            // 8. Create Course Assignments if none exist
            if (!context.CourseAssignments.Any())
            {
                context.CourseAssignments.AddRange(
                    new CourseAssignment { TeacherId = 1, CourseId = 1, ClassId = 1 },
                    new CourseAssignment { TeacherId = 2, CourseId = 2, ClassId = 1 },
                    new CourseAssignment { TeacherId = 1, CourseId = 3, ClassId = 2 }
                );
                await context.SaveChangesAsync();
            }

            // 9. Create Timetables if none exist
            if (!context.Timetables.Any())
            {
                context.Timetables.AddRange(
                    new Timetable { CourseAssignmentId = 1, DayOfWeek = "Monday", StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) },
                    new Timetable { CourseAssignmentId = 1, DayOfWeek = "Wednesday", StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) },
                    new Timetable { CourseAssignmentId = 2, DayOfWeek = "Tuesday", StartTime = TimeSpan.FromHours(11), EndTime = TimeSpan.FromHours(12) },
                    new Timetable { CourseAssignmentId = 3, DayOfWeek = "Thursday", StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(16) }
                );
                await context.SaveChangesAsync();
            }

            // 10. Create Attendance Records if none exist
            if (!context.AttendanceRecords.Any())
            {
                var today = DateTime.Today;
                context.AttendanceRecords.AddRange(
                    new Attendance { StudentId = 1, CourseId = 1, Date = today, Status = "Present" },
                    new Attendance { StudentId = 2, CourseId = 1, Date = today, Status = "Present" },
                    new Attendance { StudentId = 1, CourseId = 2, Date = today.AddDays(-1), Status = "Absent" },
                    new Attendance { StudentId = 2, CourseId = 2, Date = today.AddDays(-1), Status = "Present" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}