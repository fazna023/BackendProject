using System.ComponentModel.DataAnnotations;

namespace BackendProject2.Dto
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
