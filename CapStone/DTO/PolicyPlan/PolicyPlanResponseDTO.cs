using CapStone.Models;

namespace CapStone.DTO.PolicyPlan
{
    public class PolicyPlanResponseDTO
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;


        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; }

        public int DurationYears { get; set; }

        public string TermsConditions { get; set; } = string.Empty;

        public PlanStatus PlanStatus { get; set; }

        public int ProductId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}