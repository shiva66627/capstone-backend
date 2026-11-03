using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum Role
    {
        Admin,
        Staff,
        Customer
    }

    public enum ActiveStatusEnum
    {
        InActive,
        Active
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;

        public string? EmailOtpHash { get; set; }

        public DateTime? EmailOtpExpiry { get; set; }
        public DateTime? EmailOtpLastSentAt { get; set; }

        public int EmailOtpAttempts { get; set; } = 0;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        [Required]
        public ActiveStatusEnum ActiveStatus { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public Customer? Customer { get; set; }

        public ICollection<ClaimHistory> ClaimHistories { get; set; } = new List<ClaimHistory>();
    }
}