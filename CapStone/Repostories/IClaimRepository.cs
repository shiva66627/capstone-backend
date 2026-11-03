using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IClaimRepository
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim?> GetByIdAsync(int id);
        Task<IEnumerable<Claim>> FindAsync(Expression<Func<Claim, bool>> predicate);
        Task AddAsync(Claim entity);
        Task UpdateAsync(Claim entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId);

        Task<IEnumerable<Claim>> GetClaimsByCustomerAsync(int customerId);

        Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status);

        Task<Claim?> GetByClaimNumberAsync(int claimNumber);

        Task<(IEnumerable<Claim> Data, int TotalCount)> GetFilteredClaimsAsync(CapStone.DTO.Claim.ClaimFilterDTO filter);
    }
}