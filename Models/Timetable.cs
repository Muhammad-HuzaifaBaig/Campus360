using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Timetable
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("CourseAssignment")]
    public int CourseAssignmentId { get; set; }
    public CourseAssignment CourseAssignment { get; set; }

    [Required]
    public string DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}