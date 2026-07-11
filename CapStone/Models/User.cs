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
        Active,
        InActive
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

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        [Required]
        public ActiveStatusEnum ActiveStatus { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public Customer? Customer { get; set; }

        public ICollection<ClaimHistory> ClaimHistories { get; set; } = new List<ClaimHistory>();
    }
}