using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IClaimHistoryRepository
    {
        Task<IEnumerable<ClaimHistory>> GetAllAsync();
        Task<ClaimHistory?> GetByIdAsync(int id);
        Task<IEnumerable<ClaimHistory>> FindAsync(Expression<Func<ClaimHistory, bool>> predicate);
        Task AddAsync(ClaimHistory entity);
        Task UpdateAsync(ClaimHistory entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<IEnumerable<ClaimHistory>> GetHistoryByClaimAsync(int claimId);

        Task<(IEnumerable<ClaimHistory> Data, int TotalCount)> GetFilteredHistoryAsync(CapStone.DTO.ClaimHistory.ClaimHistoryFilterDTO filter);
    }
}