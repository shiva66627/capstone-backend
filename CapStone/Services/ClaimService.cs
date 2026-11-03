using CapStone.DTO.Claim;
using CapStone.DTO.Common;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(
            IClaimRepository claimRepository,
            IPolicyRepository policyRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ClaimService> _logger)
        {
            _claimRepository = claimRepository;
            _policyRepository = policyRepository;
            _httpContextAccessor = httpContextAccessor;
            this._logger = _logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimResponseDTO>>> GetAllClaimsAsync()
        {
            try
            {
                var claims = await _claimRepository.GetAllAsync();

                var result = claims.Select(MapToClaimResponseDTO);

                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = true,
                    Message = "Claims retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<ClaimResponseDTO>>> GetPagedClaimsAsync(ClaimFilterDTO filter)
        {
            try
            {
                var (claims, totalRecords) = await _claimRepository.GetFilteredClaimsAsync(filter);

                var result = claims.Select(MapToClaimResponseDTO);

                var paginatedData = new PaginatedResponseDTO<ClaimResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<ClaimResponseDTO>>
                {
                    Success = true,
                    Message = "Claims retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<ClaimResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<ClaimResponseDTO>> GetClaimByIdAsync(int id)
        {
            try
            {
                var claim = await _claimRepository.GetByIdAsync(id);

                if (claim == null)
                {
                    return new ApiResponseDTO<ClaimResponseDTO>
                    {
                        Success = false,
                        Message = "Claim not found."
                    };
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (claim.Policy?.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized claim {ClaimId}", currentUserId, id);
                        throw new ForbiddenAccessException("You are not authorized to view this claim.");
                    }
                }

                var result = MapToClaimResponseDTO(claim);

                return new ApiResponseDTO<ClaimResponseDTO>
                {
                    Success = true,
                    Message = "Claim retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetClaimById.");
                return new ApiResponseDTO<ClaimResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimResponseDTO>>> GetClaimsByCustomerAsync(int customerId)
        {
            try
            {
                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    
              
                }

                var claims = await _claimRepository.GetClaimsByCustomerAsync(customerId);

                // If customer is requesting, verify they own these claims
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (claims.Any(c => c.Policy?.Customer?.UserId != currentUserId))
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized claims for customer: {CustomerId}", currentUserId, customerId);
                        throw new ForbiddenAccessException("You are not authorized to view these claims.");
                    }
                }

                var result = claims.Select(MapToClaimResponseDTO);

                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = true,
                    Message = "Customer claims retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetClaimsByCustomer.");
                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> SubmitClaimAsync(ClaimRequestDTO request)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(request.PolicyId);

                if (policy == null)
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Policy not found." };
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (policy.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to submit claim for unauthorized policy: {PolicyId}", currentUserId, request.PolicyId);
                        return new ApiResponseDTO<string> { Success = false, Message = "You are not authorized to raise a claim for this policy." };
                    }
                }

                if (request.SupportingDocuments == null || request.SupportingDocuments.Count == 0)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Claim submission attempted without supporting documents for Policy: {PolicyId}", request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "Claim must have at least one supporting document reference." };
                }

                if (policy.PolicyStatus != PolicyStatus.Active)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Claim submission attempted for inactive policy: {PolicyId}", request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "Claim can be submitted only for active policies." };
                }

                                if (policy.PolicyPlan == null)
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Policy plan details not found." };
                }

                if (request.IncidentDate.Date < policy.StartDate.Date)
                {
                    _logger.LogWarning("LOG-015: Incident date {IncidentDate} prior to policy start date for Policy: {PolicyId}", request.IncidentDate, request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = $"Incident date cannot be prior to the policy purchase date ({policy.StartDate:dd-MMM-yyyy})." };
                }

                var waitingPeriodEndDate = policy.StartDate.Date.AddDays(20);
                if (request.IncidentDate.Date < waitingPeriodEndDate)
                {
                    _logger.LogWarning("LOG-015: Incident date {IncidentDate} within 20-day waiting period for Policy: {PolicyId}", request.IncidentDate, request.PolicyId);
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = $"Incident occurred within the initial 20-day waiting period. Incident date must be on or after {waitingPeriodEndDate:dd-MMM-yyyy}."
                    };
                }

                if (request.IncidentDate.Date > policy.EndDate.Date)
                {
                    _logger.LogWarning("LOG-015: Incident date {IncidentDate} after policy end date for Policy: {PolicyId}", request.IncidentDate, request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "Incident date cannot be after the policy expiration date." };
                }

                if (request.ClaimAmount > policy.PolicyPlan.CoverageAmount)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Claim amount {Amount} exceeds coverage amount {Coverage} for Policy: {PolicyId}", request.ClaimAmount, policy.PolicyPlan.CoverageAmount, request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "Claim amount exceeds the policy coverage amount." };
                }

                int claimNumber = new Random().Next(100000, 999999);

                while (await _claimRepository.GetByClaimNumberAsync(claimNumber) != null)
                {
                    claimNumber = new Random().Next(100000, 999999);
                }

                var claim = new CapStone.Models.Claim
                {
                    ClaimNumber = claimNumber,
                    PolicyId = request.PolicyId,
                    ClaimAmount = request.ClaimAmount,
                    ClaimReason = request.ClaimReason,
                    IncidentDate = request.IncidentDate,
                    ClaimStatus = ClaimStatus.Pending,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                // Map Supporting Documents to ClaimDocument entities
                foreach (var docRef in request.SupportingDocuments)
                {
                    var isUrl = Uri.TryCreate(docRef, UriKind.Absolute, out var uriResult) 
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    
                    var doc = new ClaimDocument
                    {
                        DocumentName = System.IO.Path.GetFileName(docRef) ?? "Supporting Document",
                        DocumentType = isUrl ? "URL" : "Text",
                        Field = docRef,
                        DocumentReference = isUrl ? DocumentReference.Url : DocumentReference.ReferenceText,
                        UploadedDate = DateTime.UtcNow
                    };
                    claim.ClaimDocuments.Add(doc);
                }

                await _claimRepository.AddAsync(claim);
                await _claimRepository.SaveChangesAsync();

                // Record history
                var userIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                int.TryParse(userIdClaim, out int userId);
                var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Customer";

                var history = new ClaimHistory
                {
                    ClaimId = claim.ClaimId,
                    PreviousStatus = ClaimStatus.Pending,
                    NewStatus = ClaimStatus.Pending,
                    Remarks = "Claim submitted successfully.",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = userName,
                    UserId = userId
                };
                claim.ClaimHistories.Add(history);
                await _claimRepository.UpdateAsync(claim);
                await _claimRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-009: Claim submission successful. ClaimNumber: {ClaimNo}, PolicyId: {PolicyId}", claimNumber, request.PolicyId);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Claim submitted successfully.",
                    Data = claim.ClaimNumber.ToString()
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Claim submission.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> ReviewClaimAsync(ClaimReviewDTO request)
        {
            try
            {
                var claim = await _claimRepository.GetByIdAsync(request.ClaimId);

                if (claim == null)
                {
                    throw new ResourceNotFoundException("Claim not found.");
                }

                if (claim.ClaimStatus != ClaimStatus.Pending)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to review already processed claim: {ClaimId}", request.ClaimId);
                    throw new ClaimModificationException("Only pending claims can be reviewed.");
                }

                claim.AgentRemarks = request.AgentRemarks;
                claim.UpdatedDate = DateTime.UtcNow;

                var userIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                int.TryParse(userIdClaim, out int userId);
                var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Agent";

                // Create a ClaimHistory entry for the review/recommendation
                var history = new ClaimHistory
                {
                    ClaimId = claim.ClaimId,
                    PreviousStatus = claim.ClaimStatus,
                    NewStatus = claim.ClaimStatus, // Status might only change on decision
                    Remarks = $"Recommended Status: {request.RecommendedStatus}. Agent Remarks: {request.AgentRemarks}",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = userName,
                    UserId = userId
                };
                claim.ClaimHistories.Add(history);

                await _claimRepository.UpdateAsync(claim);
                await _claimRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-010: Claim review completed. ClaimId: {ClaimId}, Remarks: {Remarks}", request.ClaimId, request.AgentRemarks);
                _logger.LogInformation("LOG-011: Claim recommendation submitted. ClaimId: {ClaimId}, Recommendation: {RecStatus}", request.ClaimId, request.RecommendedStatus);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Claim reviewed successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Claim review.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> DecideClaimAsync(ClaimDecisionDTO request)
        {
            try
            {
                var claim = await _claimRepository.GetByIdAsync(request.ClaimId);

                if (claim == null)
                {
                    throw new ResourceNotFoundException("Claim not found.");
                }

                if (claim.ClaimStatus != ClaimStatus.Pending)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to make final decision on already processed claim: {ClaimId}", request.ClaimId);
                    throw new ClaimModificationException("Claim has already been processed.");
                }

                if (request.ClaimStatus == ClaimStatus.Approved &&
                    !string.IsNullOrWhiteSpace(claim.AgentRemarks) &&
                    (claim.AgentRemarks.Contains("reject", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("re-upload", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("reupload", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("missing", StringComparison.OrdinalIgnoreCase)) &&
                    !claim.AgentRemarks.Contains("[Re-uploaded]", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("LOG-015: Admin attempted to approve claim flagged by staff: {ClaimId}", request.ClaimId);
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Cannot approve this claim: Staff has rejected or requested document re-upload. Customer must re-upload documents before approval."
                    };
                }

                var previousStatus = claim.ClaimStatus;
                claim.ClaimStatus = request.ClaimStatus;
                claim.AdminRemarks = request.AdminRemarks;
                claim.UpdatedDate = DateTime.UtcNow;

                var userIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                int.TryParse(userIdClaim, out int userId);
                var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";

                var history = new ClaimHistory
                {
                    ClaimId = claim.ClaimId,
                    PreviousStatus = previousStatus,
                    NewStatus = claim.ClaimStatus,
                    Remarks = $"Claim decision made by Admin. Status: {request.ClaimStatus}. Remarks: {request.AdminRemarks}",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = userName,
                    UserId = userId
                };
                claim.ClaimHistories.Add(history);

                await _claimRepository.UpdateAsync(claim);
                await _claimRepository.SaveChangesAsync();

                if (request.ClaimStatus == ClaimStatus.Approved)
                {
                    _logger.LogInformation("LOG-012: Final claim approval successful. ClaimId: {ClaimId}", request.ClaimId);
                }
                else
                {
                    _logger.LogInformation("LOG-013: Final claim rejection successful. ClaimId: {ClaimId}", request.ClaimId);
                }

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = request.ClaimStatus == ClaimStatus.Approved
                        ? "Claim approved successfully."
                        : "Claim rejected successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Claim decision.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private static ClaimResponseDTO MapToClaimResponseDTO(CapStone.Models.Claim c)
        {
            var agentHistory = c.ClaimHistories?
                .OrderByDescending(h => h.UpdatedDate)
                .FirstOrDefault(h => (!string.IsNullOrEmpty(h.Remarks) && (h.Remarks.Contains("Agent Remarks") || h.Remarks.StartsWith("Recommended Status:")))
                                  || (h.User != null && h.User.Role == Role.Staff));

            return new ClaimResponseDTO
            {
                ClaimId = c.ClaimId,
                ClaimNumber = c.ClaimNumber,
                ClaimAmount = c.ClaimAmount,
                ClaimReason = c.ClaimReason,
                AgentRemarks = c.AgentRemarks,
                AdminRemarks = c.AdminRemarks,
                IncidentDate = c.IncidentDate,
                ClaimStatus = c.ClaimStatus,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy?.PolicyNumber.ToString() ?? string.Empty,
                CustomerName = c.Policy?.Customer?.User?.UserName ?? string.Empty,
                AgentName = agentHistory?.UpdatedBy ?? agentHistory?.User?.UserName ?? (string.IsNullOrEmpty(c.AgentRemarks) ? null : "Agent"),
                CreatedDate = c.CreatedDate,
               
            };
        }
    }
}