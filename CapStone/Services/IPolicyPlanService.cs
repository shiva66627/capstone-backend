using CapStone.DTO.Common;
using CapStone.DTO.PolicyPlan;

namespace CapStone.Services
{
    public interface IPolicyPlanService
    {
        Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetAllPlansAsync();

        Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetAvailablePlansAsync();

        Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetPlansByProductAsync(int productId);

        Task<ApiResponseDTO<PolicyPlanResponseDTO>> GetPlanByIdAsync(int id);

        Task<ApiResponseDTO<string>> AddPlanAsync(PolicyPlanRequestDTO request);

        Task<ApiResponseDTO<string>> UpdatePlanAsync(int id, PolicyPlanRequestDTO request);

        Task<ApiResponseDTO<string>> DeactivatePlanAsync(int id);
    }
}