using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IPolicyPlanRepository
    {
        Task<IEnumerable<PolicyPlan>> GetAllAsync();
        Task<PolicyPlan?> GetByIdAsync(int id);
        Task<IEnumerable<PolicyPlan>> FindAsync(Expression<Func<PolicyPlan, bool>> predicate);
        Task AddAsync(PolicyPlan entity);
        Task UpdateAsync(PolicyPlan entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<PolicyPlan?> GetByPlanNameAsync(string planName);

        Task<IEnumerable<PolicyPlan>> GetPlansByProductAsync(int productId);

        Task<IEnumerable<PolicyPlan>> GetAvailablePlansAsync();

        Task<bool> PlanExistsAsync(int planId);

        Task<(IEnumerable<PolicyPlan> Data, int TotalCount)> GetFilteredPlansAsync(CapStone.DTO.PolicyPlan.PlanFilterDTO filter);
    }
}