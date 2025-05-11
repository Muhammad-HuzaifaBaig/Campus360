using System.ComponentModel.DataAnnotations;

namespace Campus360.ViewModels{
    public class AttendanceViewModel{
        [Required]
        public int CourseId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public required Dictionary<int,bool> AttendanceRecords { get; set; }
    }
}