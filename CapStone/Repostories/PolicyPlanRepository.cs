using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class PolicyPlanRepository : GenericRepository<PolicyPlan>, IPolicyPlanRepository
    {
        public PolicyPlanRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<PolicyPlan?> GetByPlanNameAsync(string planName)
        {
            return await _context.PolicyPlans
                .FirstOrDefaultAsync(p => p.PlanName == planName);
        }

        public async Task<IEnumerable<PolicyPlan>> GetPlansByProductAsync(int productId)
        {
            return await _context.PolicyPlans
                .Where(p => p.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PolicyPlan>> GetAvailablePlansAsync()
        {
            return await _context.PolicyPlans
                .Where(p => p.PlanStatus == PlanStatus.Available)
                .ToListAsync();
        }

        public async Task<bool> PlanExistsAsync(int planId)
        {
            return await _context.PolicyPlans
                .AnyAsync(p => p.PlanId == planId);
        }
    }
}