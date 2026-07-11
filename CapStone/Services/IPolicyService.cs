using CapStone.DTO.Common;
using CapStone.DTO.Policy;

namespace CapStone.Services
{
    public interface IPolicyService
    {
        Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetAllPoliciesAsync();

        Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetPoliciesByCustomerAsync(int customerId);

        Task<ApiResponseDTO<PolicyResponseDTO>> GetPolicyByIdAsync(int id);

        Task<ApiResponseDTO<string>> PurchasePolicyAsync(PurchasePolicyRequestDTO request);

        Task<ApiResponseDTO<string>> IssuePolicyAsync(IssuePolicyRequestDTO request);

        Task<ApiResponseDTO<string>> CancelPolicyAsync(int id);
    }
}