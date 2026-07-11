using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IClaimHistoryRepository : IGenericRepository<ClaimHistory>
    {
        Task<IEnumerable<ClaimHistory>> GetHistoryByClaimAsync(int claimId);
    }
}