using CapStone.DTO.Common;
using CapStone.DTO.Customer;

namespace CapStone.Services
{
    public interface ICustomerService
    {
        Task<ApiResponseDTO<IEnumerable<CustomerResponseDTO>>> GetAllCustomersAsync();

        Task<ApiResponseDTO<PaginatedResponseDTO<CustomerResponseDTO>>> GetPagedCustomersAsync(CustomerFilterDTO filter);

        Task<ApiResponseDTO<CustomerResponseDTO>> GetCustomerByIdAsync(int id);

        Task<ApiResponseDTO<CustomerResponseDTO>> GetCustomerByUserIdAsync(int userId);

        Task<ApiResponseDTO<string>> AddCustomerAsync(CustomerRequestDTO request);

        Task<ApiResponseDTO<string>> UpdateCustomerAsync(int id, CustomerRequestDTO request);

        Task<ApiResponseDTO<string>> DeleteCustomerAsync(int id);
    }
}