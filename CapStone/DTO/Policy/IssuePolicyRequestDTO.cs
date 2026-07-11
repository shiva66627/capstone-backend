using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.Policy
{
    public class IssuePolicyRequestDTO
    {
        [Required]
        public int PolicyNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public decimal PremiumPaid { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public PolicyStatus PolicyStatus { get; set; } = PolicyStatus.Active;
    }
}