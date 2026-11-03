using CapStone.DTO.Common;
using CapStone.DTO.PremiumPayment;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly IPremiumPaymentRepository _paymentRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PremiumPaymentService> _logger;

        public PremiumPaymentService(
            IPremiumPaymentRepository paymentRepository,
            IPolicyRepository policyRepository,
            ICustomerRepository customerRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PremiumPaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _policyRepository = policyRepository;
            _customerRepository = customerRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();

                var result = payments.Select(MapToPaymentResponseDTO);

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<PremiumPaymentResponseDTO>>> GetPagedPaymentsAsync(PaymentFilterDTO filter)
        {
            try
            {
                var (payments, totalRecords) = await _paymentRepository.GetFilteredPaymentsAsync(filter);

                var result = payments.Select(MapToPaymentResponseDTO);

                var paginatedData = new PaginatedResponseDTO<PremiumPaymentResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                return new ApiResponseDTO<PaginatedResponseDTO<PremiumPaymentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PremiumPaymentResponseDTO>> GetPaymentByIdAsync(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);

                if (payment == null)
                {
                    throw new ResourceNotFoundException("Payment not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (payment.Policy?.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized payment {PaymentId}", currentUserId, id);
                        throw new ForbiddenAccessException("You are not authorized to view this payment.");
                    }
                }

                var result = MapToPaymentResponseDTO(payment);

                return new ApiResponseDTO<PremiumPaymentResponseDTO>
                {
                    Success = true,
                    Message = "Payment retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetPaymentById.");
                return new ApiResponseDTO<PremiumPaymentResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetPaymentsByPolicyAsync(int policyId)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(policyId);
                if (policy == null)
                {
                    return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>> { Success = false, Message = "Policy not found." };
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (policy.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized policy payments: {PolicyId}", currentUserId, policyId);
                        throw new ForbiddenAccessException("You are not authorized to view payments for this policy.");
                    }
                }

                var payments = await _paymentRepository.GetPaymentsByPolicyAsync(policyId);

                var result = payments.Select(MapToPaymentResponseDTO);

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetPaymentsByPolicy.");
                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetPaymentsByCustomerAsync(int customerId)
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
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized customer payments: {CustomerId}", currentUserId, customerId);
                        throw new ForbiddenAccessException("You are not authorized to view payments for this customer.");
                    }
                }

                var payments = await _paymentRepository.GetPaymentsByCustomerAsync(customerId);

                var result = payments.Select(MapToPaymentResponseDTO);

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Customer payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetPaymentsByCustomer.");
                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> MakePaymentAsync(PremiumPaymentRequestDTO request)
        {
            try
            {
                // Check whether policy exists
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
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to make payment for unauthorized policy: {PolicyId}", currentUserId, request.PolicyId);
                        return new ApiResponseDTO<string> { Success = false, Message = "You are not authorized to make payment for this policy." };
                    }
                }
                // checking amount
                var expectedAmount = policy.PolicyPlan?.PremiumAmount;

                if (expectedAmount == null)
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Premium amount is not configured for this policy." };
                }

                if (request.Amount != expectedAmount.Value)
                {
                    _logger.LogWarning(
                        "LOG-015: Invalid payment amount. PolicyId: {PolicyId}, Expected: {Expected}, Received: {Received}",
                        request.PolicyId,
                        expectedAmount.Value,
                        request.Amount);

                    return new ApiResponseDTO<string>
                    { Success = false, Message = $"Invalid payment amount. Expected amount is {expectedAmount.Value}." };
                }


                // Check policy status
                if (policy.PolicyStatus == PolicyStatus.Cancelled || policy.PolicyStatus == PolicyStatus.Expired)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to make payment for inactive/cancelled policy: {PolicyId}", request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "Cannot make payments for a cancelled or expired policy." };
                }

                // Calculate current next due date before accepting new payment
                var currentNextDueDate = PolicyService.CalculateNextPremiumDueDate(policy);

                int intervalMonths = policy.PolicyPlan?.PremiumType switch
                {
                    PremiumType.Monthly => 1,
                    PremiumType.Quarterly => 3,
                    PremiumType.HalfYearly => 6,
                    PremiumType.Yearly => 12,
                    _ => 1
                };

                int durationYears = policy.PolicyPlan?.DurationYears > 0 ? policy.PolicyPlan.DurationYears : 1;
                int totalInstallments = (durationYears * 12) / intervalMonths;
                int successfulPaymentsCount = policy.PremiumPayments?
                    .Count(p => p.PaymentStatus == PaymentStatus.Success) ?? 0;

                if (successfulPaymentsCount >= totalInstallments || currentNextDueDate == null)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Policy {PolicyId} is already fully paid.", request.PolicyId);
                    return new ApiResponseDTO<string> { Success = false, Message = "All premium payments for this policy have already been completed." };
                }

                // If customer has already made at least one payment, block payments before the next payment due date
                if (successfulPaymentsCount > 0 && currentNextDueDate.HasValue)
                {
                    if (DateTime.UtcNow.Date < currentNextDueDate.Value.Date)
                    {
                        _logger.LogWarning("LOG-015: Business rule violation. Duplicate payment attempt for Policy {PolicyId}. Next due date: {DueDate}", request.PolicyId, currentNextDueDate.Value);
                        return new ApiResponseDTO<string> { Success = false, Message = $"Duplicate payment is not allowed for the current cycle. Next payment can only be made on or after {currentNextDueDate.Value:yyyy-MM-dd}." };
                    }
                }

                // Check transaction reference already exists
                if (await _paymentRepository.TransactionExistsAsync(request.TransactionReferenceNo))
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Duplicate transaction reference: {Ref}", request.TransactionReferenceNo);
                    return new ApiResponseDTO<string> { Success = false, Message = "Transaction Reference Number already exists." };
                }

                // Create payment
                var payment = new PremiumPayment
                {
                    Amount = request.Amount,
                    PaymentMode = request.PaymentMode,
                    TransactionReferenceNo = request.TransactionReferenceNo,
                    PaymentStatus = PaymentStatus.Success,
                    PaymentDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    PolicyId = request.PolicyId
                };

                await _paymentRepository.AddAsync(payment);

                // Update policy paid amount
                policy.PremiumPaid += request.Amount;
                policy.UpdatedDate = DateTime.UtcNow;

                // Activate policy after successful payment
                if (policy.PolicyStatus == PolicyStatus.Pending)
                {
                    policy.PolicyStatus = PolicyStatus.Active;
                }

                await _policyRepository.UpdateAsync(policy);

                await _paymentRepository.SaveChangesAsync();
                await _policyRepository.SaveChangesAsync();

                policy.PremiumPayments ??= new List<PremiumPayment>();
                policy.PremiumPayments.Add(payment);
                var nextDueDate = PolicyService.CalculateNextPremiumDueDate(policy);
                var nextDueMsg = nextDueDate.HasValue 
                    ? $" Next premium payment is due on {nextDueDate.Value:yyyy-MM-dd}." 
                    : " All premium payments for this policy term have been completed.";

                _logger.LogInformation("LOG-008: Payment record creation successful. PolicyId: {PolicyId}, Amount: {Amount}, Ref: {Ref}", request.PolicyId, request.Amount, request.TransactionReferenceNo);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = $"Payment recorded successfully.{nextDueMsg}",
                    Data = nextDueDate?.ToString("yyyy-MM-dd")
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                _logger.LogError(ex, "LOG-016: Unexpected system error during MakePayment.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<NextPremiumPaymentDTO>> GetNextPremiumPaymentDetailsAsync(int policyId)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(policyId);
                if (policy == null)
                {
                    return new ApiResponseDTO<NextPremiumPaymentDTO> { Success = false, Message = "Policy not found." };
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (policy.Customer?.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized next premium payment for policy: {PolicyId}", currentUserId, policyId);
                        return new ApiResponseDTO<NextPremiumPaymentDTO> { Success = false, Message = "You are not authorized to view payment details for this policy." };
                    }
                }
                var expectedAmount = policy.PolicyPlan?.PremiumAmount;

                if (expectedAmount == null)
                {
                    return new ApiResponseDTO<NextPremiumPaymentDTO> { Success = false, Message = "Premium amount is not configured for this policy." };
                }

               

                int intervalMonths = policy.PolicyPlan?.PremiumType switch
                {
                    PremiumType.Monthly => 1,
                    PremiumType.Quarterly => 3,
                    PremiumType.HalfYearly => 6,
                    PremiumType.Yearly => 12,
                    _ => 1
                };

                int durationYears = policy.PolicyPlan?.DurationYears > 0 ? policy.PolicyPlan.DurationYears : 1;
                int totalInstallments = (durationYears * 12) / intervalMonths;
                var successfulPayments = policy.PremiumPayments?
                    .Where(p => p.PaymentStatus == PaymentStatus.Success)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToList();
                int paymentsCount = successfulPayments?.Count ?? 0;
                var lastPayment = successfulPayments?.FirstOrDefault();

                var nextDueDate = PolicyService.CalculateNextPremiumDueDate(policy);

                var result = new NextPremiumPaymentDTO
                {
                    PolicyId = policy.PolicyId,
                    PolicyNumber = policy.PolicyNumber,
                    CustomerName = policy.Customer?.User?.UserName ?? string.Empty,
                    PlanName = policy.PolicyPlan?.PlanName ?? string.Empty,
                    PremiumAmount = policy.PolicyPlan?.PremiumAmount ?? 0,
                    PremiumType = policy.PolicyPlan?.PremiumType.ToString() ?? string.Empty,
                    NextPremiumDueDate = nextDueDate,
                    LastPaymentDate = lastPayment?.PaymentDate,
                    TotalPremiumPaid = policy.PremiumPaid,
                    PaymentsCompleted = paymentsCount,
                    TotalInstallments = totalInstallments,
                    IsFullyPaid = paymentsCount >= totalInstallments
                };

                return new ApiResponseDTO<NextPremiumPaymentDTO>
                {
                    Success = true,
                    Message = "Next premium payment details retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Handled in ApiResponseDTO
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetNextPremiumPaymentDetails.");
                return new ApiResponseDTO<NextPremiumPaymentDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private static PremiumPaymentResponseDTO MapToPaymentResponseDTO(PremiumPayment p)
        {
            return new PremiumPaymentResponseDTO
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMode = p.PaymentMode,
                PaymentStatus = p.PaymentStatus,
                TransactionReferenceNo = p.TransactionReferenceNo,
                PaymentDate = p.PaymentDate,
                PolicyNumber = p.Policy?.PolicyNumber.ToString() ?? string.Empty,
                PolicyId = p.PolicyId,
                CustomerId = p.Policy?.CustomerId ?? 0,
                CustomerName = p.Policy?.Customer?.User?.UserName ?? string.Empty,
                NextPremiumDueDate = p.Policy != null ? PolicyService.CalculateNextPremiumDueDate(p.Policy) : null
            };
        }
    }
}
  

