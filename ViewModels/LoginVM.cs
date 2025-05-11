using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Campus360.ViewModels{
    public class LoginVM{
        [Required]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [MinLength(5)]
        [Required]
        public string Password{ get; set; }
    }
}