using CapStone.Models;

namespace CapStone.DTO.PolicyPlan
{
    public class PolicyPlanResponseDTO
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Numeric product type: 0=Health, 1=Motor, 2=Life, 3=Travel
        /// </summary>
        public int ProductType { get; set; }

        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; }

        public int DurationYears { get; set; }


        public PlanStatus PlanStatus { get; set; }

        public int ProductId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
