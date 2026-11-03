using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IPremiumPaymentRepository
    {
        Task<IEnumerable<PremiumPayment>> GetAllAsync();
        Task<PremiumPayment?> GetByIdAsync(int id);
        Task<IEnumerable<PremiumPayment>> FindAsync(Expression<Func<PremiumPayment, bool>> predicate);
        Task AddAsync(PremiumPayment entity);
        Task UpdateAsync(PremiumPayment entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyAsync(int policyId);

        Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerAsync(int customerId);

        Task<PremiumPayment?> GetByTransactionReferenceAsync(string transactionReference);

        Task<bool> TransactionExistsAsync(string transactionReference);

        Task<(IEnumerable<PremiumPayment> Data, int TotalCount)> GetFilteredPaymentsAsync(CapStone.DTO.PremiumPayment.PaymentFilterDTO filter);
    }
}