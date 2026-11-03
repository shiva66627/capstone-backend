using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class PolicyPlanRepository : IPolicyPlanRepository
    {
        private readonly CapstoneDbContext _context;

        public PolicyPlanRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PolicyPlan>> GetAllAsync()
        {
            return await _context.PolicyPlans.Include(p => p.InsuranceProduct).ToListAsync();
        }

        public async Task<PolicyPlan?> GetByIdAsync(int id)
        {
            return await _context.PolicyPlans.Include(p => p.InsuranceProduct).FirstOrDefaultAsync(p => p.PlanId == id);
        }

        public async Task<IEnumerable<PolicyPlan>> FindAsync(Expression<Func<PolicyPlan, bool>> predicate)
        {
            return await _context.PolicyPlans.Include(p => p.InsuranceProduct).Where(predicate).ToListAsync();
        }

        public async Task AddAsync(PolicyPlan entity)
        {
            await _context.PolicyPlans.AddAsync(entity);
        }

        public Task UpdateAsync(PolicyPlan entity)
        {
            _context.PolicyPlans.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.PolicyPlans.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PolicyPlan?> GetByPlanNameAsync(string planName)
        {
            return await _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .FirstOrDefaultAsync(p => p.PlanName == planName);
        }

        public async Task<IEnumerable<PolicyPlan>> GetPlansByProductAsync(int productId)
        {
            return await _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .Where(p => p.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PolicyPlan>> GetAvailablePlansAsync()
        {
            return await _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .Where(p => p.PlanStatus == PlanStatus.Available)
                .ToListAsync();
        }

        public async Task<bool> PlanExistsAsync(int planId)
        {
            return await _context.PolicyPlans
                .AnyAsync(p => p.PlanId == planId);
        }

        public async Task<(IEnumerable<PolicyPlan> Data, int TotalCount)> GetFilteredPlansAsync(CapStone.DTO.PolicyPlan.PlanFilterDTO filter)
        {
            var query = _context.PolicyPlans.Include(p => p.InsuranceProduct).AsQueryable();

            if (filter.ProductId.HasValue)
                query = query.Where(p => p.ProductId == filter.ProductId.Value);

            if (filter.IsActive.HasValue)
            {
                var status = filter.IsActive.Value ? PlanStatus.Available : PlanStatus.NotAvailable;
                query = query.Where(p => p.PlanStatus == status);
            }

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "planname" => query.OrderBy(p => p.PlanName),
                    "premiumamount" => query.OrderBy(p => p.PremiumAmount),
                    "coverageamount" => query.OrderBy(p => p.CoverageAmount),
                    _ => query.OrderBy(p => p.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "planname" => query.OrderByDescending(p => p.PlanName),
                    "premiumamount" => query.OrderByDescending(p => p.PremiumAmount),
                    "coverageamount" => query.OrderByDescending(p => p.CoverageAmount),
                    _ => query.OrderByDescending(p => p.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}