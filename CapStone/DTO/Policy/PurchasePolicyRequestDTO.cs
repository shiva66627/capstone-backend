using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Policy
{
    public class PurchasePolicyRequestDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
    }
}