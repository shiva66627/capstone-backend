using CapStone.DTO.Common;
using CapStone.Models;

namespace CapStone.DTO.Claim
{
    public class ClaimFilterDTO : PaginationRequestDTO
    {
        public ClaimStatus? ClaimStatus { get; set; }
        public int? CustomerId { get; set; }
    }
}
