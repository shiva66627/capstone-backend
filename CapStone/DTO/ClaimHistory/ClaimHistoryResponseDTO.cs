using CapStone.Models;

namespace CapStone.DTO.ClaimHistory
{
    public class ClaimHistoryResponseDTO
    {
        public int HistoryId { get; set; }

        public ClaimStatus Status { get; set; }

       

        public string Remarks { get; set; } = string.Empty;

      

        public string UpdatedBy { get; set; } = string.Empty;

        public int ClaimId { get; set; }

        public int UserId { get; set; }
    }
}
