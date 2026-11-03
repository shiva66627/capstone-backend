using CapStone.Models;

namespace CapStone.DTO.Customer
{
    public class CustomerResponseDTO
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public string NomineeName { get; set; } = string.Empty;

        public string NomineeRelation { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}