using Campus360.Data.Enum;
using Campus360.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Campus360.Services
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;
        
        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetStudentIdsForAttendance(int courseId)
        {
            Semester semester = await _context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => c.Semester).FirstOrDefaultAsync();

            return await _context.Students
                .Where(s => s.Semester == semester)
                .OrderBy(s => s.Id)
                .Select(s => s.Id)
                .ToListAsync();
        }

        public async Task<Dictionary<int, List<DateTime>>> GetCoursesForAttendance(string userId)
        {
            DateTime startSem = new DateTime(2025, 4, 6);
            DateTime today = DateTime.Now.Date;

            // Get teacher ID
            var teacherId = await _context.Teachers
                .Where(t => t.UserId == userId)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (teacherId == 0)
                return new Dictionary<int, List<DateTime>>();

            // Get all courses for the teacher (not just current day)
            var courses = await _context.Timetables
                .Include(t => t.CourseAssignment)
                .ThenInclude(ca => ca.Course)
                .Where(t => t.CourseAssignment.TeacherId == teacherId)
                .Select(t => t.CourseAssignment.Course)
                .Distinct()
                .ToListAsync();

            Dictionary<int, List<DateTime>> res = new Dictionary<int, List<DateTime>>();

            // Preload timetable data for all days
            var timetableData = await _context.Timetables
                .Where(t => t.CourseAssignment.TeacherId == teacherId)
                .GroupBy(t => new { t.CourseAssignment.CourseId, t.DayOfWeek })
                .Select(g => new 
                {
                    CourseId = g.Key.CourseId,
                    Day = g.Key.DayOfWeek,
                    PeriodCount = g.Count()
                })
                .ToListAsync();

            // Preload student counts for each course
            var studentCounts = await _context.CourseAssignments
                .Where(cs => courses.Select(c => c.Id).Contains(cs.CourseId))
                .GroupBy(cs => cs.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var course in courses)
            {
                List<DateTime> missingDates = new List<DateTime>();
                var courseTimetable = timetableData.Where(t => t.CourseId == course.Id).ToList();
                int studentsInCourse = studentCounts.FirstOrDefault(sc => sc.CourseId == course.Id)?.Count ?? 0;

                for (DateTime date = startSem; date <= today ; date = date.AddDays(1))
                {
                    string dayOfWeek = date.DayOfWeek.ToString();
                    var daySchedule = courseTimetable.FirstOrDefault(t => t.Day == dayOfWeek);

                    if (daySchedule == null || studentsInCourse == 0)
                        continue; // No classes scheduled or no students

                    int requiredPeriods = daySchedule.PeriodCount;
                    int expectedRecords = requiredPeriods * studentsInCourse;

                    // Count actual attendance records for this course-date
                    int actualRecords = await _context.AttendanceRecords
                        .CountAsync(ar => ar.CourseId == course.Id && ar.Date == date);

                    if (actualRecords < expectedRecords)
                    {
                        missingDates.Add(date);
                    }
                }

                res.Add(course.Id, missingDates);
            }

            return res;
        }

        public async Task<List<Timetable>> GetTimetable(string userId)
        {
            var teacher = await _context.Teachers.Where(t => t.UserId == userId).FirstOrDefaultAsync();
            return await _context.Timetables
                .Include(t => t.CourseAssignment)
                .ThenInclude(ca => ca.Course)
                .Where(t => t.CourseAssignment.TeacherId == teacher.Id)
                .ToListAsync();
        }

        public async Task<bool> InsertAttendance(List<Attendance> attendanceList)
        {
            try
            {
                await _context.AttendanceRecords.AddRangeAsync(attendanceList);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}