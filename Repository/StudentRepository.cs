using Campus360.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Campus360.Services{
    public class StudentRepository : IStudentRepository{
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context){
            _context = context;
        }


        public async Task<List<AttendanceSummaryViewModel>> GetAttendance(string userId){
            var student = await _context.Students.Where(s => s.UserId == userId).FirstOrDefaultAsync();

            if (student == null){
                return new List<AttendanceSummaryViewModel>(); // or throw exception
                }

            return await _context.Database.SqlQueryRaw<AttendanceSummaryViewModel>(
                @"SELECT 
                    Name AS CourseName,
                    COUNT(Status) AS TotalClasses,
                    COUNT(CASE WHEN Status = 'Present' THEN 1 END) AS Presents,
                    CASE 
                        WHEN COUNT(Status) = 0 THEN 0
                        ELSE ROUND(COUNT(CASE WHEN Status = 'Present' THEN 1 END) * 100.0 / COUNT(Status), 2)
                    END AS Percentage 
                FROM Courses C 
                JOIN AttendanceRecords A ON C.Id = A.CourseId
                WHERE A.StudentId = @p0
                GROUP BY Name",
                student.Id
            ).ToListAsync();
        }

        public async Task<List<TimetableVM>> GetTimetable(string userId){
            var student = _context.Students.Where(s=> s.UserId ==  userId).FirstOrDefault();

            if (student == null){
                return new List<TimetableVM>(); // or throw exception
            }

            var timetable = await _context.Database.SqlQueryRaw<TimetableVM>(
                @"SELECT Name AS CourseName,DayOfWeek AS Day,StartTime,EndTime
                FROM Timetables T JOIN CourseAssignments CA ON T.CourseAssignmentId = CA.Id
                JOIN Courses C
                ON CA.CourseId = C.Id
                WHERE C.Semester = @p0",
                student.Id
            ).ToListAsync();
            return timetable;
        }
    }
}