using CapStone.DTO.Common;
using CapStone.Models;

namespace CapStone.DTO.Policy
{
    public class PolicyFilterDTO : PaginationRequestDTO
    {
        public PolicyStatus? PolicyStatus { get; set; }
        public int? CustomerId { get; set; }
    }
}
