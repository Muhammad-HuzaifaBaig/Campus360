using Campus360.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseAssignment> CourseAssignments { get; set; }
    public DbSet<Timetable> Timetables { get; set; }
    public DbSet<Attendance> AttendanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add unique constraints, composite keys, etc. here
        builder.Entity<Attendance>()
            .HasIndex(a => new { a.StudentId, a.CourseId, a.Date })
            .IsUnique();
    }
}