using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class PremiumPaymentRepository : IPremiumPaymentRepository
    {
        private readonly CapstoneDbContext _context;

        public PremiumPaymentRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PremiumPayment>> GetAllAsync()
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PolicyPlan)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PremiumPayments)
                .ToListAsync();
        }

        public async Task<PremiumPayment?> GetByIdAsync(int id)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PolicyPlan)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PremiumPayments)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<IEnumerable<PremiumPayment>> FindAsync(Expression<Func<PremiumPayment, bool>> predicate)
        {
            return await _context.PremiumPayments.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(PremiumPayment entity)
        {
            await _context.PremiumPayments.AddAsync(entity);
        }

        public Task UpdateAsync(PremiumPayment entity)
        {
            _context.PremiumPayments.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.PremiumPayments.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyAsync(int policyId)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PolicyPlan)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PremiumPayments)
                .Where(p => p.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerAsync(int customerId)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PolicyPlan)
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol!.PremiumPayments)
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

        public async Task<(IEnumerable<PremiumPayment> Data, int TotalCount)> GetFilteredPaymentsAsync(CapStone.DTO.PremiumPayment.PaymentFilterDTO filter)
        {
            var query = _context.PremiumPayments
                .Include(p => p.Policy).ThenInclude(pol => pol!.Customer).ThenInclude(c => c!.User)
                .Include(p => p.Policy).ThenInclude(pol => pol!.PolicyPlan)
                .Include(p => p.Policy).ThenInclude(pol => pol!.PremiumPayments)
                .AsQueryable();

            if (filter.PolicyId.HasValue)
                query = query.Where(p => p.PolicyId == filter.PolicyId.Value);

            if (filter.PaymentStatus.HasValue)
                query = query.Where(p => p.PaymentStatus == filter.PaymentStatus.Value);

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "amount" => query.OrderBy(p => p.Amount),
                    "paymentdate" => query.OrderBy(p => p.PaymentDate),
                    _ => query.OrderBy(p => p.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "amount" => query.OrderByDescending(p => p.Amount),
                    "paymentdate" => query.OrderByDescending(p => p.PaymentDate),
                    _ => query.OrderByDescending(p => p.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}