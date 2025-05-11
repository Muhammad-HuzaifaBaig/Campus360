using Campus360.Data.Enum;

namespace Campus360.Services{
    public interface ITeacherRepository{
        public Task<List<int>> GetStudentIdsForAttendance(int courseId);
        public Task<bool> InsertAttendance(List<Attendance> attendance);
        public Task<List<Timetable>> GetTimetable(string userId);
        public Task<Dictionary<int, List<DateTime>>> GetCoursesForAttendance(string userId);
    }
}