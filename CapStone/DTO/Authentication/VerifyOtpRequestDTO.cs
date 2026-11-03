
using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Authentication
{
    public class VerifyOtpRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "OTP must contain exactly 6 digits.")]
        public string Otp { get; set; } = string.Empty;
    }
}
