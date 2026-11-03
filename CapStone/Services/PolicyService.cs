using CapStone.DTO.Common;
using CapStone.DTO.Policy;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPolicyPlanRepository _planRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PolicyService> _logger;

        public PolicyService(
            IPolicyRepository policyRepository,
            ICustomerRepository customerRepository,
            IPolicyPlanRepository planRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PolicyService> logger)
        {
            _policyRepository = policyRepository;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetAllPoliciesAsync()
        {
            try
            {
                var policies = await _policyRepository.GetAllAsync();

                var result = policies.Select(MapToPolicyResponseDTO);

                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = true,
                    Message = "Policies retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<PolicyResponseDTO>>> GetPagedPoliciesAsync(PolicyFilterDTO filter)
        {
            try
            {
                var (policies, totalRecords) = await _policyRepository.GetFilteredPoliciesAsync(filter);

                var result = policies.Select(MapToPolicyResponseDTO);

                var paginatedData = new PaginatedResponseDTO<PolicyResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<PolicyResponseDTO>>
                {
                    Success = true,
                    Message = "Policies retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<PolicyResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PolicyResponseDTO>> GetPolicyByIdAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    throw new ResourceNotFoundException("Policy not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (policy.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized policy: {PolicyId}", currentUserId, id);
                        throw new ForbiddenAccessException("You are not authorized to view this policy.");
                    }
                }

                return new ApiResponseDTO<PolicyResponseDTO>
                {
                    Success = true,
                    Message = "Policy retrieved successfully.",
                    Data = MapToPolicyResponseDTO(policy)
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetPolicyById.");
                return new ApiResponseDTO<PolicyResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetPoliciesByCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    throw new ResourceNotFoundException("Customer not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (customer.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized policies for customer: {CustomerId}", currentUserId, customerId);
                        throw new ForbiddenAccessException("You are not authorized to view policies for this customer.");
                    }
                }

                var policies = await _policyRepository.GetPoliciesByCustomerAsync(customerId);

                var result = policies.Select(MapToPolicyResponseDTO);

                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = true,
                    Message = "Customer policies retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetPoliciesByCustomer.");
                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> PurchasePolicyAsync(PurchasePolicyRequestDTO request)
        {
            try
            {
                var userIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Unauthorized" };
                }

                var customer = await _customerRepository.GetByUserIdAsync(userId);
                if (customer == null ||
                    string.IsNullOrWhiteSpace(customer.City) || customer.City == "Not Updated" ||
                    string.IsNullOrWhiteSpace(customer.Address) || customer.Address == "Not Updated" ||
                    string.IsNullOrWhiteSpace(customer.NomineeName) || customer.NomineeName == "Not Updated" ||
                    string.IsNullOrWhiteSpace(customer.Pincode) || customer.Pincode == "000000")
                {
                    _logger.LogWarning("LOG-015: Customer {UserId} attempted policy purchase without completing KYC profile.", userId);
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Incomplete Profile: Please complete your Customer Profile (City, Address, Pincode & Nominee) before purchasing a policy."
                    };
                }

                var plan = await _planRepository.GetByIdAsync(request.PlanId);

                if (plan == null)
                {
                    throw new ResourceNotFoundException("Policy plan not found.");
                }

                if (plan.PlanStatus != PlanStatus.Available)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to purchase inactive plan: {PlanId}", request.PlanId);
                    throw new BusinessRuleViolationException("Only active plans can be purchased.");
                }

                var random = new Random();
                int policyNumber;

                do
                {
                    policyNumber = random.Next(100000, 999999);
                }
                while (await _policyRepository.PolicyExistsAsync(policyNumber));

                var policy = new Policy
                {
                    PolicyNumber = policyNumber,
                    CustomerId = customer.CustomerId,
                    PlanId = request.PlanId,
                    PremiumPaid = 0,
                    StartDate = request.StartDate,
                    EndDate = request.StartDate.AddYears(plan.DurationYears),
                    PolicyStatus = PolicyStatus.Pending,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _policyRepository.AddAsync(policy);
                await _policyRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-006: Policy purchase successful. Policy: {PolicyNo}, Customer: {CustomerId}", policyNumber, customer.CustomerId);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy purchased successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during PurchasePolicy.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> IssuePolicyAsync(IssuePolicyRequestDTO request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

                if (customer == null)
                {
                    throw new ResourceNotFoundException("Customer not found.");
                }

                var plan = await _planRepository.GetByIdAsync(request.PlanId);

                if (plan == null)
                {
                    throw new ResourceNotFoundException("Policy plan not found.");
                }

                if (plan.PlanStatus != PlanStatus.Available)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to issue inactive plan: {PlanId}", request.PlanId);
                    throw new BusinessRuleViolationException("Only active plans can be issued.");
                }

                var random = new Random();
                int policyNumber;
                do
                {
                    policyNumber = random.Next(100000, 999999);
                }
                while (await _policyRepository.PolicyExistsAsync(policyNumber));

                var policy = new Policy
                {
                    PolicyNumber = policyNumber,
                    CustomerId = request.CustomerId,
                    PlanId = request.PlanId,
                    PremiumPaid = 0, // Should be paid later
                    StartDate = request.StartDate,
                    EndDate = request.StartDate.AddYears(plan.DurationYears),
                    PolicyStatus = PolicyStatus.Active, // Or Pending based on business rules
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _policyRepository.AddAsync(policy);
                await _policyRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-007: Policy issuance successful. Policy: {PolicyNo}, Customer: {CustomerId}", policyNumber, request.CustomerId);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy issued successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during IssuePolicy.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> CancelPolicyAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    throw new ResourceNotFoundException("Policy not found.");
                }

                policy.PolicyStatus = PolicyStatus.Cancelled;
                policy.UpdatedDate = DateTime.UtcNow;

                await _policyRepository.UpdateAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy cancelled successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> ApprovePolicyAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    throw new ResourceNotFoundException("Policy not found.");
                }

                policy.PolicyStatus = PolicyStatus.Active;
                policy.UpdatedDate = DateTime.UtcNow;

                await _policyRepository.UpdateAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy approved successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> RejectPolicyAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    throw new ResourceNotFoundException("Policy not found.");
                }

                policy.PolicyStatus = PolicyStatus.Cancelled; // Or whatever status means rejected. Cancelled makes sense.
                policy.UpdatedDate = DateTime.UtcNow;

                await _policyRepository.UpdateAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy rejected successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public static PolicyResponseDTO MapToPolicyResponseDTO(Policy p)
        {
            return new PolicyResponseDTO
            {
                PolicyId = p.PolicyId,
                PolicyNumber = p.PolicyNumber,
                CustomerName = p.Customer?.User?.UserName ?? string.Empty,
                PlanName = p.PolicyPlan?.PlanName ?? string.Empty,
                ProductType = p.PolicyPlan?.InsuranceProduct?.ProductType.ToString() ?? string.Empty,
                CoverageAmount = p.PolicyPlan?.CoverageAmount ?? 0,
                PremiumAmount = p.PolicyPlan?.PremiumAmount ?? 0,
                PremiumType = p.PolicyPlan?.PremiumType ?? PremiumType.Monthly,
                TotalPremiumPaid = p.PremiumPaid,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                PolicyStatus = p.PolicyStatus,
                CustomerId = p.CustomerId,
                PlanId = p.PlanId,
                CreatedDate = p.CreatedDate,
                NextPremiumDueDate = CalculateNextPremiumDueDate(p)
            };
        }


        public static DateTime? CalculateNextPremiumDueDate(Policy policy)
        {
            if (policy.PolicyPlan == null)
                return null;

            if (policy.PolicyStatus == PolicyStatus.Cancelled ||
                policy.PolicyStatus == PolicyStatus.Expired)
                return null;

            int intervalMonths = policy.PolicyPlan.PremiumType switch
            {
                PremiumType.Monthly => 1,
                PremiumType.Quarterly => 3,
                PremiumType.HalfYearly => 6,
                PremiumType.Yearly => 12,
                _ => 1
            };

            int durationYears = policy.PolicyPlan.DurationYears > 0
                ? policy.PolicyPlan.DurationYears
                : 1;

            int totalInstallments = (durationYears * 12) / intervalMonths;

            int successfulPaymentsCount = policy.PremiumPayments?
                .Count(p => p.PaymentStatus == PaymentStatus.Success) ?? 0;

            if (successfulPaymentsCount >= totalInstallments)
                return null;

            // Calculate using the original anchor date
            DateTime startDate = policy.StartDate;

            int installmentNumber = successfulPaymentsCount;

            DateTime nextDueDate = startDate.AddMonths(
                installmentNumber * intervalMonths);

            if (nextDueDate > policy.EndDate)
                return null;

            return nextDueDate;
        }

    }
}