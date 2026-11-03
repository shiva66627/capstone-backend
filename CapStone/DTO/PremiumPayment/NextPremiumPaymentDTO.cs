namespace CapStone.DTO.PremiumPayment
{
    public class NextPremiumPaymentDTO
    {
        public int PolicyId { get; set; }
        public int PolicyNumber { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public decimal PremiumAmount { get; set; }
        public string PremiumType { get; set; } = string.Empty;
        public DateTime? NextPremiumDueDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal TotalPremiumPaid { get; set; }
        public int PaymentsCompleted { get; set; }
        public int TotalInstallments { get; set; }
        public bool IsFullyPaid { get; set; }
    }
}
