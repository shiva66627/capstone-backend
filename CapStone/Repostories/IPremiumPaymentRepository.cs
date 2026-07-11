using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IPremiumPaymentRepository : IGenericRepository<PremiumPayment>
    {
        Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyAsync(int policyId);

        Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerAsync(int customerId);

        Task<PremiumPayment?> GetByTransactionReferenceAsync(string transactionReference);

        Task<bool> TransactionExistsAsync(string transactionReference);
    }
}