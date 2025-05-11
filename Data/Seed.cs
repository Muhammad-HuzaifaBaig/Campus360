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
            User teacher1 = null, teacher2 = null, teacher3 = null, teacher4 = null, teacher5 = null;
            if (await userManager.FindByEmailAsync("Rohail.Qamar@campus360.com") == null)
            {
                teacher1 = new User
                {
                    UserName = "Rohail.Qamar@campus360.com",
                    Email = "Rohail.Qamar@campus360.com",
                    FullName = "Rohail Qamar",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher1, "Teacher@123");
                await userManager.AddToRoleAsync(teacher1, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher1.Id });
            }

            if (await userManager.FindByEmailAsync("Umer.Farooq@campus360.com") == null)
            {
                teacher2 = new User
                {
                    UserName = "Umer.Farooq@campus360.com",
                    Email = "Umer.Farooq@campus360.com",
                    FullName = "Umer Farooq",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher2, "Teacher@123");
                await userManager.AddToRoleAsync(teacher2, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher2.Id });
            }
            if (await userManager.FindByEmailAsync("Asma.Ashraf@campus360.com") == null)
            {
                teacher3 = new User
                {
                    UserName = "Asma.Ashraf@campus360.com",
                    Email = "Asma.Ashraf@campus360.com",
                    FullName = "Asma Ashraf",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher3, "Teacher@123");
                await userManager.AddToRoleAsync(teacher3, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher3.Id });
            }
            if (await userManager.FindByEmailAsync("Wajeeh.Uddin@campus360.com") == null)
            {
                teacher4 = new User
                {
                    UserName = "Syed.ZulfiqarUllahJafari@campus360.com",
                    Email = "Syed.ZulfiqarUllahJafari@campus360.com",
                    FullName = "Wajeeh Uddin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher4, "Teacher@123");
                await userManager.AddToRoleAsync(teacher4, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher4.Id });
            }
            if (await userManager.FindByEmailAsync("Syed.ZulfiqarUllahJafari@campus360.com") == null)
            {
                teacher5 = new User
                {
                    UserName = "Syed.ZulfiqarUllahJafari@campus360.com",
                    Email = "Syed.ZulfiqarUllahJafari@campus360.com",
                    FullName = "Syed Zulfiqar Ullah Jafari",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher5, "Teacher@123");
                await userManager.AddToRoleAsync(teacher5, "Teacher");

                context.Teachers.Add(new Teacher { UserId = teacher5.Id });
            }
            // 5. Create Classes if none exist
            if (!context.Classes.Any())
            {
                context.Classes.AddRange(
                    new Class { Capacity = 51 },
                    new Class { Capacity = 52 },
                    new Class { Capacity = 53 },
                    new Class { Capacity = 54 },
                    new Class { Capacity = 55 },
                    new Class { Capacity = 49 },
                    new Class { Capacity = 48 },
                    new Class { Capacity = 49 }
                );
                await context.SaveChangesAsync();
            }

            // 6. Create Students
            User student1 = null, student2 = null, student3 = null, student4 = null, student5 = null;
            if (await userManager.FindByEmailAsync("Uzaim.Naseem@campus360.com") == null)
            {
                student1 = new User
                {
                    UserName = "Uzaim.Naseem@campus360.com",
                    Email = "Uzaim.Naseem@campus360.com",
                    FullName = "Uzaim Naseem",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student1, "Student@123");
                await userManager.AddToRoleAsync(student1, "Student");

                context.Students.Add(new Student
                {
                    UserId = student1.Id,
                    ClassId = 4,
                    Semester = Semester.Fourth
                });
            }

            if (await userManager.FindByEmailAsync("Hammad.Khan@campus360.com") == null)
            {
                student2 = new User
                {
                    UserName = "Hammad.Khan@campus360.com",
                    Email = "Hammad.Khan@campus360.com",
                    FullName = "Hammad Khan",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student2, "Student@123");
                await userManager.AddToRoleAsync(student2, "Student");

                context.Students.Add(new Student
                {
                    UserId = student2.Id,
                    ClassId = 4,
                    Semester = Semester.Fourth
                });
            }
            if (await userManager.FindByEmailAsync("Ahmed.Hasan@campus360.com") == null)
            {
                student3 = new User
                {
                    UserName = "Ahmed.Hasan@campus360.com",
                    Email = "Ahmed.Hasan@campus360.com",
                    FullName = "Ahmed.Hasan",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student3, "Student@123");
                await userManager.AddToRoleAsync(student3, "Student");

                context.Students.Add(new Student
                {
                    UserId = student3.Id,
                    ClassId = 4,
                    Semester = Semester.Fourth
                });
            }
            if (await userManager.FindByEmailAsync("Hamza.Baig@campus360.com") == null)
            {
                student4 = new User
                {
                    UserName = "Hamza.Baig@campus360.com",
                    Email = "Hamza.Baig@campus360.com",
                    FullName = "Hamza Baig",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student4, "Student@123");
                await userManager.AddToRoleAsync(student4, "Student");

                context.Students.Add(new Student
                {
                    UserId = student4.Id,
                    ClassId = 4,
                    Semester = Semester.Fourth
                });
            }
            if (await userManager.FindByEmailAsync("Raahim.Rizwan@campus360.com") == null)
            {
                student5 = new User
                {
                    UserName = "Raahim.Rizwan@campus360.com",
                    Email = "Raahim.Rizwan@campus360.com",
                    FullName = "Raahim Rizwan",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student5, "Student@123");
                await userManager.AddToRoleAsync(student5, "Student");

                context.Students.Add(new Student
                {
                    UserId = student5.Id,
                    ClassId = 4,
                    Semester = Semester.Fourth
                });
            }

            //7.Create Courses if none exist
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

           // 8.Create Course Assignments if none exist
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