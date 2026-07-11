using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IClaimRepository : IGenericRepository<Claim>
    {
        Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId);

        Task<IEnumerable<Claim>> GetClaimsByCustomerAsync(int customerId);

        Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status);

        Task<Claim?> GetByClaimNumberAsync(int claimNumber);
    }
}