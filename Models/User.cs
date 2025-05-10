using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class User : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }

}