using CapStone.DTO.Common;
using CapStone.Models;

namespace CapStone.DTO.PremiumPayment
{
    public class PaymentFilterDTO : PaginationRequestDTO
    {
        public int? PolicyId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
