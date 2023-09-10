using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username Required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Username should contain only letters")]
        [StringLength(20, ErrorMessage = "Name should not exceed 20 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()-_+=]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter, one digit, and may include special characters")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}