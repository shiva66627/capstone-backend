using CapStone.DTO.Common;
using CapStone.DTO.PremiumPayment;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly IPremiumPaymentRepository _paymentRepository;
        private readonly IPolicyRepository _policyRepository;

        public PremiumPaymentService(
            IPremiumPaymentRepository paymentRepository,
            IPolicyRepository policyRepository)
        {
            _paymentRepository = paymentRepository;
            _policyRepository = policyRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();

                var result = payments.Select(p => new PremiumPaymentResponseDTO
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentMode = p.PaymentMode,
                    PaymentStatus = p.PaymentStatus,
                    TransactionReferenceNo = p.TransactionReferenceNo,
                    PaymentDate = p.PaymentDate,
                    PolicyId = p.PolicyId
                });

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
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
                    return new ApiResponseDTO<PremiumPaymentResponseDTO>
                    {
                        Success = false,
                        Message = "Payment not found."
                    };
                }

                var result = new PremiumPaymentResponseDTO
                {
                    PaymentId = payment.PaymentId,
                    Amount = payment.Amount,
                    PaymentMode = payment.PaymentMode,
                    PaymentStatus = payment.PaymentStatus,
                    TransactionReferenceNo = payment.TransactionReferenceNo,
                    PaymentDate = payment.PaymentDate,
                    PolicyId = payment.PolicyId
                };

                return new ApiResponseDTO<PremiumPaymentResponseDTO>
                {
                    Success = true,
                    Message = "Payment retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
                var payments = await _paymentRepository.GetPaymentsByPolicyAsync(policyId);

                var result = payments.Select(p => new PremiumPaymentResponseDTO
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentMode = p.PaymentMode,
                    PaymentStatus = p.PaymentStatus,
                    TransactionReferenceNo = p.TransactionReferenceNo,
                    PaymentDate = p.PaymentDate,
                    PolicyId = p.PolicyId
                });

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
                var payments = await _paymentRepository.GetPaymentsByCustomerAsync(customerId);

                var result = payments.Select(p => new PremiumPaymentResponseDTO
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentMode = p.PaymentMode,
                    PaymentStatus = p.PaymentStatus,
                    TransactionReferenceNo = p.TransactionReferenceNo,
                    PaymentDate = p.PaymentDate,
                    PolicyId = p.PolicyId
                });

                return new ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>
                {
                    Success = true,
                    Message = "Customer payments retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy not found."
                    };
                }

                // Check transaction reference already exists
                if (await _paymentRepository.TransactionExistsAsync(request.TransactionReferenceNo))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Transaction Reference Number already exists."
                    };
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

                // Activate policy after successful payment
                if (policy.PolicyStatus == PolicyStatus.Pending)
                {
                    policy.PolicyStatus = PolicyStatus.Active;
                    policy.UpdatedDate = DateTime.UtcNow;

                    await _policyRepository.UpdateAsync(policy);
                }

                await _paymentRepository.SaveChangesAsync();
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Premium payment completed successfully."
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
  

