using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class PremiumPaymentRepository : GenericRepository<PremiumPayment>, IPremiumPaymentRepository
    {
        public PremiumPaymentRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyAsync(int policyId)
        {
            return await _context.PremiumPayments
                .Where(p => p.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerAsync(int customerId)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                .Where(p => p.Policy != null && p.Policy.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<PremiumPayment?> GetByTransactionReferenceAsync(string transactionReference)
        {
            return await _context.PremiumPayments
                .FirstOrDefaultAsync(p => p.TransactionReferenceNo == transactionReference);
        }

        public async Task<bool> TransactionExistsAsync(string transactionReference)
        {
            return await _context.PremiumPayments
                .AnyAsync(p => p.TransactionReferenceNo == transactionReference);
        }
    }
}