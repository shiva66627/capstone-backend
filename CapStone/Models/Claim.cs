using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public int ClaimNumber { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal ClaimAmount { get; set; }

        [Required]
        [StringLength(500)]
        public string ClaimReason { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AgentRemarks { get; set; }

        [StringLength(500)]
        public string? AdminRemarks { get; set; }

        [Required]
        public DateTime IncidentDate { get; set; }

        [Required]
        public ClaimStatus ClaimStatus { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int PolicyId { get; set; }

        // Navigation Property
        public Policy? Policy { get; set; }

        public ICollection<ClaimDocument> ClaimDocuments { get; set; } = new List<ClaimDocument>();

        public ICollection<ClaimHistory> ClaimHistories { get; set; } = new List<ClaimHistory>();
    }
}