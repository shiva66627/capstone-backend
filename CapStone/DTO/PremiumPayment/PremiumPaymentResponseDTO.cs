using CapStone.Models;

namespace CapStone.DTO.PremiumPayment
{
    public class PremiumPaymentResponseDTO
    {
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMode PaymentMode { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string TransactionReferenceNo { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public int PolicyId { get; set; }
    }
}