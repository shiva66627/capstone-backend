using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Claim
{
    public class ClaimRequestDTO
    {
        [Required]
        public int PolicyId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal ClaimAmount { get; set; }

        [Required]
        [StringLength(500)]
        public string ClaimReason { get; set; } = string.Empty;

        [Required]
        public DateTime IncidentDate { get; set; }
    }
}