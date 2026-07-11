using CapStone.DTO.ClaimHistory;
using CapStone.DTO.Common;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class ClaimHistoryService : IClaimHistoryService
    {
        private readonly IClaimHistoryRepository _historyRepository;

        public ClaimHistoryService(IClaimHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>> GetAllHistoryAsync()
        {
            try
            {
                var history = await _historyRepository.GetAllAsync();

                var result = history.Select(h => new ClaimHistoryResponseDTO
                {
                    HistoryId = h.HistoryId,
                    PreviousStatus = h.PreviousStatus,
                    NewStatus = h.NewStatus,
                    Remarks = h.Remarks,
                    UpdatedDate = h.UpdatedDate,
                    UpdatedBy = h.UpdatedBy,
                    ClaimId = h.ClaimId,
                    UserId = h.UserId
                });

                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = true,
                    Message = "Claim history retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>> GetHistoryByClaimAsync(int claimId)
        {
            try
            {
                var history = await _historyRepository.GetHistoryByClaimAsync(claimId);

                var result = history.Select(h => new ClaimHistoryResponseDTO
                {
                    HistoryId = h.HistoryId,
                    PreviousStatus = h.PreviousStatus,
                    NewStatus = h.NewStatus,
                    Remarks = h.Remarks,
                    UpdatedDate = h.UpdatedDate,
                    UpdatedBy = h.UpdatedBy,
                    ClaimId = h.ClaimId,
                    UserId = h.UserId
                });

                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = true,
                    Message = "Claim history retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}