using Campus360.Data;
using Campus360.Data.Enum;
using Campus360.Models;
using Campus360.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Xml;

namespace Campus360.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Course Operations
        public async Task<List<Course>> GetCoursesBySemesterAsync(Semester semester)
        {
            return await _context.Courses
                .Where(c => c.Semester == semester)
                .ToListAsync();
        }
        public async Task<List<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }
        public async Task<List<Class>> GetAllClasses()
        {
            return await _context.Classes.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<bool> CreateCourseAsync(Course course)
        {
            bool courseExists = await _context.Courses
                .AnyAsync(c => c.Name == course.Name);

            if (courseExists)
                return false;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Teacher Operations
        public async Task<List<TeacherViewModel>> GetTeachersAsync()
        {
            var teacherRoleId = await _context.Roles
                .Where(r => r.Name == "Teacher") 
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(teacherRoleId))
            {
                return new List<TeacherViewModel>();
            }

            
            var teachers = await (from user in _context.Users
                                  join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                  join teacherRecord in _context.Teachers on user.Id equals teacherRecord.UserId 
                                  where userRole.RoleId == teacherRoleId
                                  select new TeacherViewModel
                                  {
                                      Id = teacherRecord.Id,         
                                      TeacherId = user.Id,        
                                      FullName = user.FullName,
                                      Email = user.Email
                                      
                                  }).ToListAsync();

            return teachers;
        }

        #endregion

        #region Assignment Operations
        public async Task<List<CourseAssignment>> GetAllAssignmentsAsync(int id)
        {
            return await _context.CourseAssignments
                .Where(ca => ca.TeacherId == id) 
                .Include(ca => ca.Teacher)
                    .ThenInclude(t => t.User) 
                .Include(ca => ca.Course)       
                .Include(ca => ca.Class)        
                .ToListAsync();
        }
        public async Task<bool> IsCourseAlreadyAssigned(int teacherId, int courseId)
        {
            return await _context.CourseAssignments
                .AnyAsync(ca => ca.TeacherId == teacherId && ca.CourseId == courseId);
        }


        public async Task AssignCourseAsync(CourseAssignment assignment)
        {
            _context.CourseAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }
        public async Task<CourseAssignment?> GetAssignmentByIdAsync(int id)
        {
            return await _context.CourseAssignments
                .Include(a => a.Teacher)
                .Include(a => a.Course)
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task RemoveAssignmentAsync(int assignmentId)
        {
            var assignment = await _context.CourseAssignments.FindAsync(assignmentId);
            if (assignment != null)
            {
                _context.CourseAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Timetable Operations
        public async Task<Dictionary<Semester, Dictionary<Class, List<Timetable>>>> GetAllTimetablesGroupedAsync()
        {
            var timetables = await _context.Timetables
                .Include(t => t.CourseAssignment)
                    .ThenInclude(ca => ca.Course)
                .Include(t => t.CourseAssignment)
                    .ThenInclude(ca => ca.Teacher)
                        .ThenInclude(t => t.User)
                .Include(t => t.CourseAssignment.Class)
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            
            var grouped = timetables
                .GroupBy(t => t.CourseAssignment.Course.Semester)
                .ToDictionary(
                    g => g.Key, 
                    g => g.GroupBy(t => t.CourseAssignment.Class)
                          .ToDictionary(cg => cg.Key, cg => cg.ToList())
                );

            return grouped;
        }


        public async Task<Timetable?> GetTimetableByIdAsync(int id)
        {
            return await _context.Timetables
                .Include(t => t.CourseAssignment)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTimetableAsync(Timetable timetable)
        {
            var courseAssignment = await _context.CourseAssignments
                .FirstOrDefaultAsync(ca => ca.Id == timetable.CourseAssignmentId);
            if (await HasTimetableConflictAsync(courseAssignment.ClassId, timetable))
            {
                throw new InvalidOperationException("Scheduling conflict detected");
            }

            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTimetableAsync(Timetable timetable)
        {

            var existing = await _context.Timetables
                .FirstOrDefaultAsync(t => t.Id == timetable.Id);

            if (existing == null)
                throw new InvalidOperationException("Timetable not found");

            // Get the classId using CourseAssignmentId
            var courseAssignment = await _context.CourseAssignments
                .FirstOrDefaultAsync(ca => ca.Id == timetable.CourseAssignmentId);

            if (courseAssignment == null)
                throw new InvalidOperationException("Invalid Course Assignment");

            if (await HasTimetableConflictAsync(courseAssignment.ClassId, timetable))
            {
                throw new InvalidOperationException("Scheduling conflict detected");
            }

            existing.CourseAssignmentId = timetable.CourseAssignmentId;
            existing.DayOfWeek = timetable.DayOfWeek;
            existing.StartTime = timetable.StartTime;
            existing.EndTime = timetable.EndTime;

            await _context.SaveChangesAsync();
        }

        private async Task<bool> HasTimetableConflictAsync(int classId , Timetable timetable)
        {
            return await _context.Timetables
                .Where(t => t.CourseAssignment != null &&
                            t.CourseAssignment.ClassId == classId &&
                            t.DayOfWeek == timetable.DayOfWeek &&
                            t.Id != timetable.Id &&
                            t.StartTime == timetable.StartTime &&
                            t.EndTime == timetable.EndTime)
                .AnyAsync();
        }


        public async Task DeleteTimetableAsync(int id)
        {
            var timetable = await _context.Timetables.FindAsync(id);
            if (timetable != null)
            {
                _context.Timetables.Remove(timetable);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

    }
}