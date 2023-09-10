 using Sample.Data;
using System.ComponentModel.DataAnnotations;

namespace Sample.Data
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }

        [Required(ErrorMessage = "Name Required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name should contain only letters and spaces")]
        [StringLength(20, ErrorMessage = "Name should not exceed 20 characters.")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Date of Birth Required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Location Required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Location should contain only letters and spaces")]
        [StringLength(15, ErrorMessage = "Location should not exceed 15 characters.")]
        public string? Location { get; set; }
        
        [Required(ErrorMessage = "ContactNumber Required")]
        [RegularExpression(@"^\+?[0-9]{1,3}-?[0-9]{4,14}$", ErrorMessage = "Invalid Contact Number")]
        public string? ContactNumber { get; set; }
        
        [Required(ErrorMessage = "Designation Required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Designation should contain only letters and spaces")]
        [StringLength(20, ErrorMessage = "Designation should not exceed 20 characters.")]
        public string? Designation { get; set; }
        
        [Required(ErrorMessage = "Dateof Join  Required")]
        public DateTime DateOfJoin { get; set; }
        
        [Required(ErrorMessage = "Password Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()-_+=]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter, one digit, and may include special characters")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}