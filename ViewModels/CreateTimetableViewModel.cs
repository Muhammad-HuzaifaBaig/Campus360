using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Campus360.ViewModels
{
    public class CreateTimetableViewModel
    {
        [Required(ErrorMessage = "Please select a course assignment")]
        [Display(Name = "Course Assignment")]
        public int CourseAssignmentId { get; set; }

        [Required(ErrorMessage = "Please select a day")]
        [Display(Name = "Day of Week")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "Please specify start time")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Please specify end time")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public List<SelectListItem> CourseAssignments { get; set; } = new();
        public List<SelectListItem> DaysOfWeek { get; set; } = new();
    }
}
