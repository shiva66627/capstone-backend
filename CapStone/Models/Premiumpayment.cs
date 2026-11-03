using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum PaymentStatus
    {
          Failed,
        Success,
     
        Pending
    }

    public enum PaymentMode
    {
        UPI,
        Card,
        NetBanking,
        Cash
    }

    public class PremiumPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMode PaymentMode { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string TransactionReferenceNo { get; set; } = string.Empty;

        // Foreign Key
        public int PolicyId { get; set; }

        // Navigation Property
        public Policy? Policy { get; set; }
    }
}