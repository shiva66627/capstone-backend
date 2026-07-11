using CapStone.Models;

namespace CapStone.DTO.User
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public Role Role { get; set; }

        public ActiveStatusEnum ActiveStatus { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}