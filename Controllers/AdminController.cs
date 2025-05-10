using Campus360.Data.Enum;
using Campus360.Models;
using Campus360.Services;
using Campus360.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Campus360.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly AppDbContext _context;
        public AdminController(IAdminRepository adminRepository, AppDbContext context)
        {
            _adminRepository = adminRepository;
            _context = context;
        }
        [HttpGet]
        public IActionResult CoursesBySemester()
        {
            var model = new CoursesBySemesterViewModel
            {
                SemesterOptions = Enum.GetValues(typeof(Semester))
                                      .Cast<Semester>()
                                      .Select(s => new SelectListItem
                                      {
                                          Value = s.ToString(),
                                          Text = s.ToString()
                                      }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CoursesBySemester(CoursesBySemesterViewModel model)
        {
            model.SemesterOptions = Enum.GetValues(typeof(Semester))
                                        .Cast<Semester>()
                                        .Select(s => new SelectListItem
                                        {
                                            Value = s.ToString(),
                                            Text = s.ToString()
                                        }).ToList();

            if (model.SelectedSemester.HasValue)
            {
                model.Courses = await _adminRepository.GetCoursesBySemesterAsync(model.SelectedSemester.Value);
            }

            return View(model);
        }
        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _adminRepository.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpGet]
        public IActionResult CreateCourse() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (!ModelState.IsValid) return View(course);

            var success = await _adminRepository.CreateCourseAsync(course);

            if (!success)
            {
                ModelState.AddModelError("", "A course with the same name already exists in any semester.");
                return View(course);
            }

            return RedirectToAction("CoursesBySemester", new { semester = course.Semester });
        }


        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _adminRepository.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course)
        {
            if (!ModelState.IsValid) return View(course);
            await _adminRepository.UpdateCourseAsync(course);
            return RedirectToAction("CourseDetails", new { id = course.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _adminRepository.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View(course); 
        }

        [HttpPost, ActionName("DeleteCourse")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _adminRepository.DeleteCourseAsync(id);
            return RedirectToAction("CoursesBySemester");
        }

        public async Task<IActionResult> AllTeachers()
        {
            var teachers = await _adminRepository.GetTeachersAsync();
            return View(teachers); 
        }


        public async Task<IActionResult> AllAssignments(int id)
        {
            var assignments = await _adminRepository.GetAllAssignmentsAsync(id);
            ViewBag.id = id;
            return View(assignments);
        }
        [HttpGet]
        public async Task<IActionResult> AssignCourse(int id)
        {
            var courses = await _adminRepository.GetAllCourses();
            var classes = await _adminRepository.GetAllClasses();
            var viewModel = new AssignCourseViewModel
            {
                TeacherId = id,
                Courses = courses,
                Classes = classes
            };
            
            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> AssignCourse(AssignCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Courses = await _adminRepository.GetAllCourses();
                model.Classes = await _adminRepository.GetAllClasses();
                return View(model);
            }

            var isDuplicate = await _adminRepository.IsCourseAlreadyAssigned(model.TeacherId, model.SelectedCourseId);

            if (isDuplicate)
            {
                ModelState.AddModelError("", "This course is already assigned to this teacher.");
                model.Courses = await _adminRepository.GetAllCourses();
                model.Classes = await _adminRepository.GetAllClasses();
                return View(model);
            }

            var assignment = new CourseAssignment
            {
                TeacherId = model.TeacherId,
                CourseId = model.SelectedCourseId,
                ClassId = model.SelectedClassId
            };

            await _adminRepository.AssignCourseAsync(assignment);
            if (model.TeacherId == 0) 
            {
                return RedirectToAction("Error"); 
            }
            return RedirectToAction("AllAssignments", new { id = model.TeacherId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
       
            var assignment = await _adminRepository.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            await _adminRepository.RemoveAssignmentAsync(id);
            return RedirectToAction("AllAssignments", new { id = assignment.TeacherId });
        }
        [HttpGet]
        public async Task<IActionResult> AllTimetables()
        {
            var timetables = await _adminRepository.GetAllTimetablesGroupedAsync();
            return View(timetables); 
        }
        [HttpGet]
        public async Task<IActionResult> CreateTimetable()
        {
            var viewModel = new CreateTimetableViewModel
            {
                CourseAssignments = await _context.CourseAssignments
                    .Include(ca => ca.Course)
                    .Include(ca => ca.Teacher)
                    .ThenInclude(t => t.User)
                    .Include(ca => ca.Class)
                    .Select(ca => new SelectListItem
                    {
                        Value = ca.Id.ToString(),
                        Text = $"{ca.Course.Name} - {ca.Teacher.User.FullName} - Class {ca.Class.Id}"
                    })
                    .ToListAsync(),

                DaysOfWeek = new List<string>
            { "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday" }
                    .Select(d => new SelectListItem { Value = d, Text = d })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTimetable(CreateTimetableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CourseAssignments = await GetCourseAssignmentSelectList();
                model.DaysOfWeek = GetDaysOfWeekSelectList();
                return View(model);
            }

            var timetable = new Timetable
            {
                CourseAssignmentId = model.CourseAssignmentId,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            if (await HasTimetableConflict(timetable))
            {
                ModelState.AddModelError("", "Scheduling conflict detected");
                model.CourseAssignments = await GetCourseAssignmentSelectList();
                model.DaysOfWeek = GetDaysOfWeekSelectList();
                return View(model);
            }

            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Timetable created successfully!";
            return RedirectToAction("AllTimetables");
        }
        [HttpGet]
        public async Task<IActionResult> EditTimetable(int id)
        {
            var timetable = await _adminRepository.GetTimetableByIdAsync(id);
            if (timetable == null)
            {
                return NotFound();
            }

            var viewModel = new EditTimetableViewModel
            {
                Id = timetable.Id,
                CourseAssignmentId = timetable.CourseAssignmentId,
                DayOfWeek = timetable.DayOfWeek,
                StartTime = timetable.StartTime,
                EndTime = timetable.EndTime,
                CourseAssignments = await GetCourseAssignmentSelectList(),
                DaysOfWeek = GetDaysOfWeekSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTimetable(EditTimetableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CourseAssignments = await GetCourseAssignmentSelectList();
                model.DaysOfWeek = GetDaysOfWeekSelectList();
                return View(model);
            }

            var timetable = new Timetable
            {
                Id = model.Id,
                CourseAssignmentId = model.CourseAssignmentId,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            try
            {
                await _adminRepository.UpdateTimetableAsync(timetable);
                TempData["SuccessMessage"] = "Timetable updated successfully!";
                return RedirectToAction("AllTimetables");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.CourseAssignments = await GetCourseAssignmentSelectList();
                model.DaysOfWeek = GetDaysOfWeekSelectList();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTimetable(int id)
        {
            try
            {
                await _adminRepository.DeleteTimetableAsync(id);
                TempData["SuccessMessage"] = "Timetable deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting timetable: " + ex.Message;
            }

            return RedirectToAction("AllTimetables");
        }
        private async Task<bool> HasTimetableConflict(Timetable timetable)
        {
            return await _context.Timetables.AnyAsync(t =>
                t.CourseAssignmentId == timetable.CourseAssignmentId &&
                t.DayOfWeek == timetable.DayOfWeek &&
                t.StartTime < timetable.EndTime &&
                t.EndTime > timetable.StartTime);
        }
        private async Task<List<SelectListItem>> GetCourseAssignmentSelectList()
        {
            return await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Teacher)
                .Include(ca => ca.Class)
                .Select(ca => new SelectListItem
                {
                    Value = ca.Id.ToString(),
                    Text = $"{ca.Course.Name} - {ca.Teacher.User.FullName} - Class {ca.Class.Id}"
                })
                .ToListAsync();
        }

        private List<SelectListItem> GetDaysOfWeekSelectList()
        {
            return new List<string>
        { "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday" }
                .Select(d => new SelectListItem { Value = d, Text = d })
                .ToList();
        }
    }
}
