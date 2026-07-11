using CapStone.Models;

namespace CapStone.DTO.Policy
{
    public class PolicyResponseDTO
    {
        public int PolicyId { get; set; }

        public int PolicyNumber { get; set; }

        public decimal PremiumPaid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public PolicyStatus PolicyStatus { get; set; }

        public int CustomerId { get; set; }

        public int PlanId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}