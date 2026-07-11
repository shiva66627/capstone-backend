using CapStone.DTO.ClaimHistory;
using CapStone.DTO.Common;

namespace CapStone.Services
{
    public interface IClaimHistoryService
    {
        Task<ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>> GetAllHistoryAsync();

        Task<ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>> GetHistoryByClaimAsync(int claimId);
    }
}