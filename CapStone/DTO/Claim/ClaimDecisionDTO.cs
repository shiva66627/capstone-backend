using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.Claim
{
    public class ClaimDecisionDTO
    {
        [Required]
        public int ClaimId { get; set; }

        [Required]
        public ClaimStatus ClaimStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string AdminRemarks { get; set; } = string.Empty;
    }
}