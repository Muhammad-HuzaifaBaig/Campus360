using Campus360.Data.Enum;
using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Class")]
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public Semester Semester { get; set; }
}