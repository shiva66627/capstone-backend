using System.ComponentModel.DataAnnotations;
using CapStone.Attributes;
using CapStone.Models;

namespace CapStone.DTO.Customer
{
    public class CustomerRequestDTO
    {
        [Required]
        public Gender Gender { get; set; }

        [Required]
        [NotFutureDate(ErrorMessage = "Date of birth cannot be in the future.")]
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
        [MinLength(10, ErrorMessage = "Nominee name must be at least 10 characters.")]
        public string NomineeName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NomineeRelation { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }
    }
}