using CapStone.DTO.ClaimHistory;
using CapStone.DTO.Common;
using CapStone.Exceptions;
using CapStone.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class ClaimHistoryService : IClaimHistoryService
    {
        private readonly IClaimHistoryRepository _historyRepository;
        private readonly IClaimRepository _claimRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClaimHistoryService> _logger;

        public ClaimHistoryService(
            IClaimHistoryRepository historyRepository,
            IClaimRepository claimRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ClaimHistoryService> logger)
        {
            _historyRepository = historyRepository;
            _claimRepository = claimRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>> GetAllHistoryAsync()
        {
            try
            {
                var history = await _historyRepository.GetAllAsync();

                var result = history.Select(h => new ClaimHistoryResponseDTO
                {
                    HistoryId = h.HistoryId,
                    Status = h.NewStatus,
                    Remarks = h.Remarks,
               
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<ClaimHistoryResponseDTO>>> GetPagedHistoryAsync(ClaimHistoryFilterDTO filter)
        {
            try
            {
                var (history, totalRecords) = await _historyRepository.GetFilteredHistoryAsync(filter);

                var result = history.Select(h => new ClaimHistoryResponseDTO
                {
                    HistoryId = h.HistoryId,
                    Status = h.NewStatus,
                    Remarks = h.Remarks,
                    UpdatedBy = h.UpdatedBy,
                    ClaimId = h.ClaimId,
                    UserId = h.UserId
                });

                var paginatedData = new PaginatedResponseDTO<ClaimHistoryResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<ClaimHistoryResponseDTO>>
                {
                    Success = true,
                    Message = "Claim history retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<ClaimHistoryResponseDTO>>
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
                var claim = await _claimRepository.GetByIdAsync(claimId);
                if (claim == null)
                {
                    throw new ResourceNotFoundException("Claim not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (claim.Policy?.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized claim history for claim: {ClaimId}", currentUserId, claimId);
                        throw new ForbiddenAccessException("You are not authorized to view claim history for this claim.");
                    }
                }

                var history = await _historyRepository.GetHistoryByClaimAsync(claimId);

                var result = history.Select(h => new ClaimHistoryResponseDTO
                {
                    HistoryId = h.HistoryId,
                  
                    Status = h.NewStatus,
                    Remarks = h.Remarks,
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetHistoryByClaim.");
                return new ApiResponseDTO<IEnumerable<ClaimHistoryResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}