using Campus360.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Teacher
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }
}
