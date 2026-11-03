using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class ClaimHistoryRepository : IClaimHistoryRepository
    {
        private readonly CapstoneDbContext _context;

        public ClaimHistoryRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClaimHistory>> GetAllAsync()
        {
            return await _context.ClaimHistories.ToListAsync();
        }

        public async Task<ClaimHistory?> GetByIdAsync(int id)
        {
            return await _context.ClaimHistories.FindAsync(id);
        }

        public async Task<IEnumerable<ClaimHistory>> FindAsync(Expression<Func<ClaimHistory, bool>> predicate)
        {
            return await _context.ClaimHistories.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(ClaimHistory entity)
        {
            await _context.ClaimHistories.AddAsync(entity);
        }

        public Task UpdateAsync(ClaimHistory entity)
        {
            _context.ClaimHistories.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ClaimHistories.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClaimHistory>> GetHistoryByClaimAsync(int claimId)
        {
            return await _context.ClaimHistories
                .Where(h => h.ClaimId == claimId)
                .OrderByDescending(h => h.UpdatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<ClaimHistory> Data, int TotalCount)> GetFilteredHistoryAsync(CapStone.DTO.ClaimHistory.ClaimHistoryFilterDTO filter)
        {
            var query = _context.ClaimHistories.AsQueryable();

            if (filter.ClaimId.HasValue)
                query = query.Where(h => h.ClaimId == filter.ClaimId.Value);

            if (!string.IsNullOrEmpty(filter.UpdatedBy))
                query = query.Where(h => h.UpdatedBy.Contains(filter.UpdatedBy));

            if (filter.Status.HasValue)
                query = query.Where(h => h.NewStatus == filter.Status.Value);

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "updatedby" => query.OrderBy(h => h.UpdatedBy),
                    "status" => query.OrderBy(h => h.NewStatus),
                    _ => query.OrderBy(h => h.UpdatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "updatedby" => query.OrderByDescending(h => h.UpdatedBy),
                    "status" => query.OrderByDescending(h => h.NewStatus),
                    _ => query.OrderByDescending(h => h.UpdatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}