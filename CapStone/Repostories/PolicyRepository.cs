using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class PolicyRepository : GenericRepository<Policy>, IPolicyRepository
    {
        public PolicyRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Policy>> GetPoliciesByCustomerAsync(int customerId)
        {
            return await _context.Policies
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Policy?> GetByPolicyNumberAsync(int policyNumber)
        {
            return await _context.Policies
                .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);
        }

        public async Task<bool> PolicyExistsAsync(int policyNumber)
        {
            return await _context.Policies
                .AnyAsync(p => p.PolicyNumber == policyNumber);
        }
    }
}