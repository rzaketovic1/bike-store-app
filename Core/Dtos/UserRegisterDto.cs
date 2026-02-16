using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Display name is required")]
        [MinLength(2, ErrorMessage = "Display name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Display name cannot exceed 50 characters")]
        public required string DisplayName { get; set; }
    }
}
