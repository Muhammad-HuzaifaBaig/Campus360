using System.ComponentModel.DataAnnotations;

namespace Campus360.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public int Capacity { get; set; }
    }
}
