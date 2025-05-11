using System.ComponentModel.DataAnnotations;

namespace Campus360.ViewModels{
    public class RegisterViewModel{
        [Required]
        public string Email { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}