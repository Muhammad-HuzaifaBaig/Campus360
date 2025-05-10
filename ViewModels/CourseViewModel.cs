using Campus360.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Campus360.ViewModels
{
    public class CourseViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public CreditHours CreditHours { get; set; }
        public Semester Semester { get; set; }
    }
}
