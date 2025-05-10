using Campus360.Data.Enum;
using Campus360.Models;
using Campus360.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Campus360.Services
{
    public interface IAdminRepository
    {
        Task<List<Course>> GetAllCourses();
        Task<List<Class>> GetAllClasses();
        Task<bool> IsCourseAlreadyAssigned(int teacherId, int courseId);
        Task<List<Course>> GetCoursesBySemesterAsync(Semester semesters);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<bool> CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
        Task<List<TeacherViewModel>> GetTeachersAsync();
        Task<List<CourseAssignment>> GetAllAssignmentsAsync(int id);
        Task AssignCourseAsync(CourseAssignment assignment);
        Task<CourseAssignment?> GetAssignmentByIdAsync(int id);
        Task RemoveAssignmentAsync(int assignmentId);
        Task<Dictionary<Semester, Dictionary<Class, List<Timetable>>>> GetAllTimetablesGroupedAsync();
        Task<Timetable?> GetTimetableByIdAsync(int id);
        Task CreateTimetableAsync(Timetable timetable);
        Task UpdateTimetableAsync(Timetable timetable);
        Task DeleteTimetableAsync(int id);
    }

    public record StudentWithDetails(
        int StudentId,
        string FullName,
        string Email,
        string ClassName,
        Semester Semester,
        List<CourseEnrollment> Enrollments);

    public record CourseEnrollment(
        string CourseName,
        string TeacherName,
        string AttendanceStatus);
}