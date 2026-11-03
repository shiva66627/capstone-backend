using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<Policy>> GetAllAsync();
        Task<Policy?> GetByIdAsync(int id);
        Task<IEnumerable<Policy>> FindAsync(Expression<Func<Policy, bool>> predicate);
        Task AddAsync(Policy entity);
        Task UpdateAsync(Policy entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<IEnumerable<Policy>> GetPoliciesByCustomerAsync(int customerId);

        Task<Policy?> GetByPolicyNumberAsync(int policyNumber);

        Task<bool> PolicyExistsAsync(int policyNumber);

        Task<(IEnumerable<Policy> Data, int TotalCount)> GetFilteredPoliciesAsync(CapStone.DTO.Policy.PolicyFilterDTO filter);
    }
}