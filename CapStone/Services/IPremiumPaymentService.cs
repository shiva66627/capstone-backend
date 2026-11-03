using CapStone.DTO.Common;
using CapStone.DTO.PremiumPayment;

namespace CapStone.Services
{
    public interface IPremiumPaymentService
    {
        Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetAllPaymentsAsync();

        Task<ApiResponseDTO<PaginatedResponseDTO<PremiumPaymentResponseDTO>>> GetPagedPaymentsAsync(PaymentFilterDTO filter);

        Task<ApiResponseDTO<PremiumPaymentResponseDTO>> GetPaymentByIdAsync(int id);

        Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetPaymentsByPolicyAsync(int policyId);

        Task<ApiResponseDTO<IEnumerable<PremiumPaymentResponseDTO>>> GetPaymentsByCustomerAsync(int customerId);

        Task<ApiResponseDTO<string>> MakePaymentAsync(PremiumPaymentRequestDTO request);
        Task<ApiResponseDTO<NextPremiumPaymentDTO>> GetNextPremiumPaymentDetailsAsync(int policyId);
    }
}