using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum PolicyStatus
    {
        Pending,
        Active,
        Expired,
        Cancelled
    }

    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        public int PolicyNumber { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PremiumPaid { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public PolicyStatus PolicyStatus { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys

        public int PlanId { get; set; }

        public int CustomerId { get; set; }

        // Navigation Properties

        public PolicyPlan? PolicyPlan { get; set; }

        public Customer? Customer { get; set; }

        public ICollection<PremiumPayment> PremiumPayments { get; set; } = new List<PremiumPayment>();

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
