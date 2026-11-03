using CapStone.DTO.Common;

namespace CapStone.DTO.PolicyPlan
{
    public class PlanFilterDTO : PaginationRequestDTO
    {
        public int? ProductId { get; set; }
        public bool? IsActive { get; set; }
    }
}
