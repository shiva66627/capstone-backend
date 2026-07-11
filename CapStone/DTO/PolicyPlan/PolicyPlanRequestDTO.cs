using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.PolicyPlan
{
    public class PolicyPlanRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } = string.Empty;

       

        [Required]
        [Range(1, double.MaxValue)]
        public decimal CoverageAmount { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal PremiumAmount { get; set; }

        [Required]
        public PremiumType PremiumType { get; set; }

        [Required]
        [Range(1, 100)]
        public int DurationYears { get; set; }

        [Required]
        [StringLength(2000)]
        public string TermsConditions { get; set; } = string.Empty;

        [Required]
        public PlanStatus PlanStatus { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}