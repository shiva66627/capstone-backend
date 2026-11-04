using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Authentication
{
    public class ResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

