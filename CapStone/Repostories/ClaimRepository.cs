using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class ClaimRepository : GenericRepository<Claim>, IClaimRepository
    {
        public ClaimRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId)
        {
            return await _context.Claims
                .Where(c => c.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByCustomerAsync(int customerId)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Where(c => c.Policy != null && c.Policy.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status)
        {
            return await _context.Claims
                .Where(c => c.ClaimStatus == status)
                .ToListAsync();
        }

        public async Task<Claim?> GetByClaimNumberAsync(int claimNumber)
        {
            return await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
        }
    }
}