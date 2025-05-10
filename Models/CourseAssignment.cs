using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CourseAssignment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Teacher")]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public Course Course { get; set; }

    [ForeignKey("Class")]
    public int ClassId { get; set; }
    public Class Class { get; set; }
}