using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum PremiumType
    {
        Monthly,
        Quarterly,
        HalfYearly,
        Yearly
    }

    public enum PlanStatus
    {
        NotAvailable,
        Available
    }

    public class PolicyPlan
    {
        [Key]
        public int PlanId { get; set; }

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

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation Property
        public InsuranceProduct? InsuranceProduct { get; set; }

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}
