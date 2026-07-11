using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IPolicyRepository : IGenericRepository<Policy>
    {
        Task<IEnumerable<Policy>> GetPoliciesByCustomerAsync(int customerId);

        Task<Policy?> GetByPolicyNumberAsync(int policyNumber);

        Task<bool> PolicyExistsAsync(int policyNumber);
    }
}