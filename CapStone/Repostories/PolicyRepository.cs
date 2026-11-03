using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly CapstoneDbContext _context;

        public PolicyRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Policy>> GetAllAsync()
        {
            return await _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c!.User)
                .Include(p => p.PolicyPlan)
                    .ThenInclude(pp => pp!.InsuranceProduct)
                .Include(p => p.PremiumPayments)
                .ToListAsync();
        }

        public async Task<Policy?> GetByIdAsync(int id)
        {
            return await _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c!.User)
                .Include(p => p.PolicyPlan)
                    .ThenInclude(pp => pp!.InsuranceProduct)
                .Include(p => p.PremiumPayments)
                .FirstOrDefaultAsync(p => p.PolicyId == id);
        }

        public async Task<IEnumerable<Policy>> FindAsync(Expression<Func<Policy, bool>> predicate)
        {
            return await _context.Policies.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Policy entity)
        {
            await _context.Policies.AddAsync(entity);
        }

        public Task UpdateAsync(Policy entity)
        {
            _context.Policies.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Policies.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Policy>> GetPoliciesByCustomerAsync(int customerId)
        {
            return await _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c!.User)
                .Include(p => p.PolicyPlan)
                    .ThenInclude(pp => pp!.InsuranceProduct)
                .Include(p => p.PremiumPayments)
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Policy?> GetByPolicyNumberAsync(int policyNumber)
        {
            return await _context.Policies
                .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);
        }

        public async Task<bool> PolicyExistsAsync(int policyNumber)
        {
            return await _context.Policies
                .AnyAsync(p => p.PolicyNumber == policyNumber);
        }

        public async Task<(IEnumerable<Policy> Data, int TotalCount)> GetFilteredPoliciesAsync(CapStone.DTO.Policy.PolicyFilterDTO filter)
        {
            var query = _context.Policies
                .Include(p => p.Customer).ThenInclude(c => c!.User)
                .Include(p => p.PolicyPlan).ThenInclude(pp => pp!.InsuranceProduct)
                .Include(p => p.PremiumPayments)
                .AsQueryable();

            if (filter.PolicyStatus.HasValue)
                query = query.Where(p => p.PolicyStatus == filter.PolicyStatus.Value);

            if (filter.CustomerId.HasValue)
                query = query.Where(p => p.CustomerId == filter.CustomerId.Value);

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "startdate" => query.OrderBy(p => p.StartDate),
                    "enddate" => query.OrderBy(p => p.EndDate),
                    "policynumber" => query.OrderBy(p => p.PolicyNumber),
                    _ => query.OrderBy(p => p.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "startdate" => query.OrderByDescending(p => p.StartDate),
                    "enddate" => query.OrderByDescending(p => p.EndDate),
                    "policynumber" => query.OrderByDescending(p => p.PolicyNumber),
                    _ => query.OrderByDescending(p => p.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}