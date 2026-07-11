using CapStone.Models;

namespace CapStone.DTO.ClaimHistory
{
    public class ClaimHistoryResponseDTO
    {
        public int HistoryId { get; set; }

        public ClaimStatus PreviousStatus { get; set; }

        public ClaimStatus NewStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

        public int ClaimId { get; set; }

        public int UserId { get; set; }
    }
}
