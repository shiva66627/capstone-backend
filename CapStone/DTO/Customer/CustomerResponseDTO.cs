namespace CapStone.DTO.Customer
{
    public class CustomerResponseDTO
    {
        public int CustomerId { get; set; }

        public DateTime DateOfBirth { get; set; }

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