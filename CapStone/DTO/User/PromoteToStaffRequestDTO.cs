using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.User
{
    public class PromoteToStaffRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
