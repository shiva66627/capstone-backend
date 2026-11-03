using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string Pincode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NomineeName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NomineeRelation { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User? User { get; set; }

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}