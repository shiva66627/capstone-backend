using CapStone.DTO.Common;
using CapStone.DTO.InsuranceProduct;

namespace CapStone.Services
{
    public interface IInsuranceProductService
    {
        Task<ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>> GetAllProductsAsync();

        Task<ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>> GetAvailableProductsAsync();

        Task<ApiResponseDTO<InsuranceProductResponseDTO>> GetProductByIdAsync(int id);

        Task<ApiResponseDTO<string>> AddProductAsync(InsuranceProductRequestDTO request);

        Task<ApiResponseDTO<string>> UpdateProductAsync(int id, InsuranceProductRequestDTO request);

        Task<ApiResponseDTO<string>> DeactivateProductAsync(int id);
    }
}