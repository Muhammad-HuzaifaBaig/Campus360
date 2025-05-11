using Campus360.ViewModels;

namespace Campus360.Services{
    public interface IStudentRepository{
        public Task<List<AttendanceSummaryViewModel>> GetAttendance(string userId);
        public Task<List<TimetableVM>> GetTimetable(string userId);
    }
}