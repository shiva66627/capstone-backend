using CapStone.DTO.Common;
using CapStone.DTO.Customer;

namespace CapStone.Services
{
    public interface ICustomerService
    {
        Task<ApiResponseDTO<IEnumerable<CustomerResponseDTO>>> GetAllCustomersAsync();

        Task<ApiResponseDTO<CustomerResponseDTO>> GetCustomerByIdAsync(int id);

        Task<ApiResponseDTO<string>> AddCustomerAsync(CustomerRequestDTO request);

        Task<ApiResponseDTO<string>> UpdateCustomerAsync(int id, CustomerRequestDTO request);

        Task<ApiResponseDTO<string>> DeleteCustomerAsync(int id);
    }
}