using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IPolicyPlanRepository : IGenericRepository<PolicyPlan>
    {
        Task<PolicyPlan?> GetByPlanNameAsync(string planName);

        Task<IEnumerable<PolicyPlan>> GetPlansByProductAsync(int productId);

        Task<IEnumerable<PolicyPlan>> GetAvailablePlansAsync();

        Task<bool> PlanExistsAsync(int planId);
    }
}