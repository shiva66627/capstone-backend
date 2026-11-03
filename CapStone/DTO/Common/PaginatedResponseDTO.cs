namespace CapStone.DTO.Common
{
    public class PaginatedResponseDTO<T>
    {
        public IEnumerable<T> Records { get; set; } = Enumerable.Empty<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool LastPageIndicator { get; set; }
        public string SortField { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;

        public PaginatedResponseDTO() { }

        public PaginatedResponseDTO(IEnumerable<T> records, int totalRecords, int currentPage, int pageSize, string sortField, string sortDirection)
        {
            Records = records;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            LastPageIndicator = CurrentPage >= TotalPages;
            SortField = sortField;
            SortDirection = sortDirection;
        }
    }
}
