using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Authentication
{
    public class ResendOtpRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
