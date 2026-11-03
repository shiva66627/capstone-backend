using CapStone.DTO.Common;
using CapStone.Models;

namespace CapStone.DTO.ClaimHistory
{
    public class ClaimHistoryFilterDTO : PaginationRequestDTO
    {
        public int? ClaimId { get; set; }
        public string? UpdatedBy { get; set; }
        public ClaimStatus? Status { get; set; }
    }
}
