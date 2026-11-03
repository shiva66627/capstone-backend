using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Claim
{
    public class ClaimReviewDTO
    {
        [Required]
        public int ClaimId { get; set; }

        [Required]
        [StringLength(500)]
        public string AgentRemarks { get; set; } = string.Empty;

        // Added to match PDF SRS: "Recommended status"
        [Required]
        public string RecommendedStatus { get; set; } = string.Empty;
    }
}