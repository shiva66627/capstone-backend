using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly CapstoneDbContext _context;

        public ClaimRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claims
                .Include(c => c.Policy)
                    .ThenInclude(p => p!.Customer)
                        .ThenInclude(cu => cu!.User)
                .Include(c => c.ClaimHistories)
                    .ThenInclude(ch => ch.User)
                .ToListAsync();
        }

        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                    .ThenInclude(p => p!.Customer)
                        .ThenInclude(cu => cu!.User)
                .Include(c => c.ClaimHistories)
                    .ThenInclude(ch => ch.User)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
        }

        public async Task<IEnumerable<Claim>> FindAsync(Expression<Func<Claim, bool>> predicate)
        {
            return await _context.Claims.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Claim entity)
        {
            await _context.Claims.AddAsync(entity);
        }

        public Task UpdateAsync(Claim entity)
        {
            _context.Claims.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Claims.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId)
        {
            return await _context.Claims
                .Where(c => c.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByCustomerAsync(int customerId)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                    .ThenInclude(p => p!.Customer)
                        .ThenInclude(cu => cu!.User)
                .Include(c => c.ClaimHistories)
                    .ThenInclude(ch => ch.User)
                .Where(c => c.Policy != null && c.Policy.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                    .ThenInclude(p => p!.Customer)
                        .ThenInclude(cu => cu!.User)
                .Include(c => c.ClaimHistories)
                    .ThenInclude(ch => ch.User)
                .Where(c => c.ClaimStatus == status)
                .ToListAsync();
        }

        public async Task<Claim?> GetByClaimNumberAsync(int claimNumber)
        {
            return await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
        }

        public async Task<(IEnumerable<Claim> Data, int TotalCount)> GetFilteredClaimsAsync(CapStone.DTO.Claim.ClaimFilterDTO filter)
        {
            var query = _context.Claims
                .Include(c => c.Policy).ThenInclude(p => p!.Customer).ThenInclude(cu => cu!.User)
                .Include(c => c.ClaimHistories).ThenInclude(ch => ch.User)
                .AsQueryable();

            if (filter.ClaimStatus.HasValue)
                query = query.Where(c => c.ClaimStatus == filter.ClaimStatus.Value);

            if (filter.CustomerId.HasValue)
                query = query.Where(c => c.Policy != null && c.Policy.CustomerId == filter.CustomerId.Value);

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "claimamount" => query.OrderBy(c => c.ClaimAmount),
                    "incidentdate" => query.OrderBy(c => c.IncidentDate),
                    "claimnumber" => query.OrderBy(c => c.ClaimNumber),
                    _ => query.OrderBy(c => c.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "claimamount" => query.OrderByDescending(c => c.ClaimAmount),
                    "incidentdate" => query.OrderByDescending(c => c.IncidentDate),
                    "claimnumber" => query.OrderByDescending(c => c.ClaimNumber),
                    _ => query.OrderByDescending(c => c.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}