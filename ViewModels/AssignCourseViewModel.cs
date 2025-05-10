using Campus360.Models;
using System.ComponentModel.DataAnnotations;

namespace Campus360.ViewModels
{
    public class AssignCourseViewModel
    {
        public int TeacherId { get; set; }

        [Required]
        public int SelectedCourseId { get; set; }

        [Required]
        public int SelectedClassId { get; set; }

        public List<Course>? Courses { get; set; }
        public List<Class>? Classes { get; set; }

    }
}
