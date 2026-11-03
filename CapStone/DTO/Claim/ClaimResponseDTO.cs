using CapStone.Models;

namespace CapStone.DTO.Claim
{
    public class ClaimResponseDTO
    {
        public int ClaimId { get; set; }

        public int ClaimNumber { get; set; }

        public decimal ClaimAmount { get; set; }

        public string ClaimReason { get; set; } = string.Empty;

        public string? AgentRemarks { get; set; }

        public string? AdminRemarks { get; set; }

        public DateTime IncidentDate { get; set; }

        public ClaimStatus ClaimStatus { get; set; }

        public int PolicyId { get; set; } // Internal ID

        // Added to match PDF SRS
        public string PolicyNumber { get; set; } = string.Empty;
        
        // Added to match PDF SRS
        public string CustomerName { get; set; } = string.Empty;

        public string? AgentName { get; set; }

        public DateTime CreatedDate { get; set; }

        
    }
}