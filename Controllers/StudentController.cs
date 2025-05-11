using Campus360.Services;
using Campus360.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Campus360.Controllers{
    public class StudentController : Controller{
        private readonly IHttpContextAccessor _accessor;
        private readonly IStudentRepository _repo;

        public StudentController(IHttpContextAccessor accessor,IStudentRepository repo){
            _accessor = accessor;
            _repo = repo;
        }

        public async Task<IActionResult> Attendance(){
            var userId = _accessor.HttpContext?.User.GetUserId();
            var attendance = await _repo.GetAttendance(userId);
            return View(attendance);
        }

        public async Task<IActionResult> Timetable(string id){
            var userId = _accessor.HttpContext?.User.GetUserId();
            var timetable = await _repo.GetTimetable(userId);
            return View(timetable);
        }
    }
}