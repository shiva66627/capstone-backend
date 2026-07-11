using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.PremiumPayment
{
    public class PremiumPaymentRequestDTO
    {
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMode PaymentMode { get; set; }

        [Required]
        public string TransactionReferenceNo { get; set; } = string.Empty;

        [Required]
        public int PolicyId { get; set; }
    }
}