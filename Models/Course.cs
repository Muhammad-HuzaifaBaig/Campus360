using Campus360.Data.Enum;
using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public CreditHours CreditHours { get; set; }
    public Semester Semester { get; set; }
}