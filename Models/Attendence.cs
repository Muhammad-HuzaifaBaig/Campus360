using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Student")]
    public int StudentId { get; set; }
    public Student Student { get; set; }

    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public Course Course { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Status { get; set; } // Present / Absent
}