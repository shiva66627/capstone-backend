using CapStone.DTO.Claim;
using CapStone.DTO.Common;

namespace CapStone.Services
{
    public interface IClaimService
    {
        Task<ApiResponseDTO<IEnumerable<ClaimResponseDTO>>> GetAllClaimsAsync();

        Task<ApiResponseDTO<PaginatedResponseDTO<ClaimResponseDTO>>> GetPagedClaimsAsync(ClaimFilterDTO filter);

        Task<ApiResponseDTO<ClaimResponseDTO>> GetClaimByIdAsync(int id);

        Task<ApiResponseDTO<IEnumerable<ClaimResponseDTO>>> GetClaimsByCustomerAsync(int customerId);

        Task<ApiResponseDTO<string>> SubmitClaimAsync(ClaimRequestDTO request);

        Task<ApiResponseDTO<string>> ReviewClaimAsync(ClaimReviewDTO request);

        Task<ApiResponseDTO<string>> DecideClaimAsync(ClaimDecisionDTO request);
    }
}