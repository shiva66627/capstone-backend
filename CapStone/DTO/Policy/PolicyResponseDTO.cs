using CapStone.Models;

namespace CapStone.DTO.Policy
{
    public class PolicyResponseDTO
    {
        public int PolicyId { get; set; }

        public int PolicyNumber { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string PlanName { get; set; } = string.Empty;

        public string ProductType { get; set; } = string.Empty;

        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public PolicyStatus PolicyStatus { get; set; }

        public decimal TotalPremiumPaid { get; set; }

        // Keeping these for internal usage if they had it
        public int CustomerId { get; set; }
        public int PlanId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? NextPremiumDueDate { get; set; }
    }
}