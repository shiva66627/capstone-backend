using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum ProductType
    {
        Health,
        Motor,
        Life,
        Travel
    }

    public enum ActiveStatus
    {
     NotAvailable,
        Available
    }

    public class InsuranceProduct
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public ProductType ProductType { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ActiveStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ICollection<PolicyPlan> PolicyPlans { get; set; } = new List<PolicyPlan>();
    }
}