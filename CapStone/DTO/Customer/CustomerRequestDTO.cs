using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Customer
{
    public class CustomerRequestDTO
    {
        [Required]
        public DateTime DateOfBirth { get; set; }

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

        [Required]
        public int UserId { get; set; }
    }
}