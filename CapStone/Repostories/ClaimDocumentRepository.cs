using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class ClaimDocumentRepository : IClaimDocumentRepository
    {
        private readonly CapstoneDbContext _context;

        public ClaimDocumentRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClaimDocument>> GetAllAsync()
        {
            return await _context.ClaimDocuments
                .Include(cd => cd.Claim)
                    .ThenInclude(c => c.Policy)
                        .ThenInclude(p => p.Customer)
                            .ThenInclude(cu => cu.User)
                .ToListAsync();
        }

        public async Task<ClaimDocument?> GetByIdAsync(int id)
        {
            return await _context.ClaimDocuments.FindAsync(id);
        }

        public async Task<IEnumerable<ClaimDocument>> FindAsync(Expression<Func<ClaimDocument, bool>> predicate)
        {
            return await _context.ClaimDocuments.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(ClaimDocument entity)
        {
            await _context.ClaimDocuments.AddAsync(entity);
        }

        public Task UpdateAsync(ClaimDocument entity)
        {
            _context.ClaimDocuments.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ClaimDocuments.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimAsync(int claimId)
        {
            return await _context.ClaimDocuments
                .Include(cd => cd.Claim)
                    .ThenInclude(c => c.Policy)
                        .ThenInclude(p => p.Customer)
                            .ThenInclude(cu => cu.User)
                .Where(cd => cd.ClaimId == claimId)
                .ToListAsync();
        }

        public async Task<ClaimDocument?> GetByDocumentNameAsync(string documentName)
        {
            return await _context.ClaimDocuments
                .FirstOrDefaultAsync(cd => cd.DocumentName == documentName);
        }
    }
}