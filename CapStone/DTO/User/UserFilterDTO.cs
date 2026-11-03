using CapStone.DTO.Common;

namespace CapStone.DTO.User
{
    public class UserFilterDTO : PaginationRequestDTO
    {
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
