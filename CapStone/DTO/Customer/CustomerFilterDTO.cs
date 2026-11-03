using CapStone.DTO.Common;

namespace CapStone.DTO.Customer
{
    public class CustomerFilterDTO : PaginationRequestDTO
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortField { get; set; } = "CreatedDate";

        public string SortDirection { get; set; } = "asc";
    }
}
