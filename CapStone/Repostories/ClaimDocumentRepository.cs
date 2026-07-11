using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class ClaimDocumentRepository : GenericRepository<ClaimDocument>, IClaimDocumentRepository
    {
        public ClaimDocumentRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimAsync(int claimId)
        {
            return await _context.ClaimDocuments
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