using CapStone.DTO.Common;

namespace CapStone.DTO.InsuranceProduct
{
    public class ProductFilterDTO : PaginationRequestDTO
    {
        public string? ProductType { get; set; }
        public bool? IsActive { get; set; }
    }
}
