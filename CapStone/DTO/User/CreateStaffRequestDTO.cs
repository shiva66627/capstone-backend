using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.User
{
    public class CreateStaffRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$")]
        public string MobileNumber { get; set; } = string.Empty;
    }
}
