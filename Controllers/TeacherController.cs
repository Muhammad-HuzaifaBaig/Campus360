using Campus360.Data.Enum;
using Campus360.Services;
using Campus360.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Campus360.Controllers{
    public class TeacherController : Controller{
        IHttpContextAccessor _accessor;
        ITeacherRepository _repo;
        public TeacherController(IHttpContextAccessor accessor,ITeacherRepository repo){
            _accessor = accessor;
            _repo = repo;
        }

        public async Task<IActionResult> Timetable(){
            var id = _accessor.HttpContext!.User.GetUserId();
            var fixtures = await _repo.GetTimetable(id);
            var fixturesVM = new List<TimetableVM>();
            foreach(var fixture in fixtures){
                var fixtureVM = new TimetableVM{
                    CourseName = fixture.CourseAssignment.Course.Name,
                    Day = fixture.DayOfWeek,
                    StartTime = fixture.StartTime,
                    EndTime = fixture.EndTime
                };
                fixturesVM.Add(fixtureVM);
            }
            return View(fixturesVM);
        }

        public async Task<IActionResult> AttendanceList(){
            var id = _accessor.HttpContext!.User.GetUserId();
           var courses =  await _repo.GetCoursesForAttendance(id);

           return View(courses);
        }

        public async Task<IActionResult> Attendance(int courseId, DateTime date)
        {
            var studentIds = await _repo.GetStudentIdsForAttendance(courseId);

            AttendanceViewModel attendanceViewModel = new AttendanceViewModel()
            {
                CourseId = courseId,
                Date = date,
                AttendanceRecords = studentIds.ToDictionary(id => id, id => false)
            };

            return View(attendanceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Attendance(AttendanceViewModel attendanceViewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Home", "Error");

            List<Attendance> attendances = new List<Attendance>();

            foreach (var attendanceRecord in attendanceViewModel.AttendanceRecords)
            {
                Attendance attendance = new Attendance()
                {
                    StudentId = attendanceRecord.Key,
                    CourseId = attendanceViewModel.CourseId,
                    Date = attendanceViewModel.Date,
                    Status = attendanceRecord.Value ? "Present" : "Absent"
                };

                attendances.Add(attendance);
            }

            await _repo.InsertAttendance(attendances);

            return RedirectToAction("AttendanceList");
        }
    }
}