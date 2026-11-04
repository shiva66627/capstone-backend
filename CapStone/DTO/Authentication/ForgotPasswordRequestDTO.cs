using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Authentication
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}

