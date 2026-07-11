using CapStone.DTO.Claim;
using CapStone.DTO.Common;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPolicyRepository _policyRepository;

        public ClaimService(
            IClaimRepository claimRepository,
            IPolicyRepository policyRepository)
        {
            _claimRepository = claimRepository;
            _policyRepository = policyRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimResponseDTO>>> GetAllClaimsAsync()
        {
            try
            {
                var claims = await _claimRepository.GetAllAsync();

                var result = claims.Select(c => new ClaimResponseDTO
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
                    CreatedDate = c.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = true,
                    Message = "Claims retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
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

                var result = new ClaimResponseDTO
                {
                    ClaimId = claim.ClaimId,
                    ClaimNumber = claim.ClaimNumber,
                    ClaimAmount = claim.ClaimAmount,
                    ClaimReason = claim.ClaimReason,
                    AgentRemarks = claim.AgentRemarks,
                    AdminRemarks = claim.AdminRemarks,
                    IncidentDate = claim.IncidentDate,
                    ClaimStatus = claim.ClaimStatus,
                    PolicyId = claim.PolicyId,
                    CreatedDate = claim.CreatedDate
                };

                return new ApiResponseDTO<ClaimResponseDTO>
                {
                    Success = true,
                    Message = "Claim retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
                var claims = await _claimRepository.GetClaimsByCustomerAsync(customerId);

                var result = claims.Select(c => new ClaimResponseDTO
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
                    CreatedDate = c.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<ClaimResponseDTO>>
                {
                    Success = true,
                    Message = "Customer claims retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy not found."
                    };
                }

                if (policy.PolicyStatus != PolicyStatus.Active)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Claim can be submitted only for active policies."
                    };
                }

                int claimNumber = new Random().Next(100000, 999999);

                while (await _claimRepository.GetByClaimNumberAsync(claimNumber) != null)
                {
                    claimNumber = new Random().Next(100000, 999999);
                }

                var claim = new Claim
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

                await _claimRepository.AddAsync(claim);
                await _claimRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Claim submitted successfully.",
                    Data = claim.ClaimNumber.ToString()
                };
            }
            catch (Exception ex)
            {
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
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Claim not found."
                    };
                }

                if (claim.ClaimStatus != ClaimStatus.Pending)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Only pending claims can be reviewed."
                    };
                }

                claim.AgentRemarks = request.AgentRemarks;
                claim.UpdatedDate = DateTime.UtcNow;

                await _claimRepository.UpdateAsync(claim);
                await _claimRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Claim reviewed successfully."
                };
            }
            catch (Exception ex)
            {
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
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Claim not found."
                    };
                }

                if (claim.ClaimStatus != ClaimStatus.Pending)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Claim has already been processed."
                    };
                }

                claim.ClaimStatus = request.ClaimStatus;
                claim.AdminRemarks = request.AdminRemarks;
                claim.UpdatedDate = DateTime.UtcNow;

                await _claimRepository.UpdateAsync(claim);
                await _claimRepository.SaveChangesAsync();

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
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
    