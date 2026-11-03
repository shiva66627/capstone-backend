using CapStone.Models;

namespace CapStone.DTO.InsuranceProduct
{
    public class InsuranceProductResponseDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public ProductType ProductType { get; set; }

        public string Description { get; set; } = string.Empty;

        public ActiveStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}