using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.PolicyPlan
{
    public class PolicyPlanRequestDTO
    {
        [Required]
        [StringLength(100)]
        [MinLength(10, ErrorMessage = "Plan name must be at least 10 characters.")]
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
        public PlanStatus PlanStatus { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}