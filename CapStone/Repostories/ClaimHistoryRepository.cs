using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class ClaimHistoryRepository : GenericRepository<ClaimHistory>, IClaimHistoryRepository
    {
        public ClaimHistoryRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<ClaimHistory>> GetHistoryByClaimAsync(int claimId)
        {
            return await _context.ClaimHistories
                .Where(h => h.ClaimId == claimId)
                .OrderByDescending(h => h.UpdatedDate)
                .ToListAsync();
        }
    }
}