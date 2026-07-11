using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public class ClaimHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public ClaimStatus PreviousStatus { get; set; }

        [Required]
        public ClaimStatus NewStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;

        // Foreign Keys

        public int ClaimId { get; set; }

        public int UserId { get; set; }

        // Navigation Properties

        public Claim? Claim { get; set; }

        public User? User { get; set; }
    }
}