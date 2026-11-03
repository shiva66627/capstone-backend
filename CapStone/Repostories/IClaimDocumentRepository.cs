using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IClaimDocumentRepository
    {
        Task<IEnumerable<ClaimDocument>> GetAllAsync();
        Task<ClaimDocument?> GetByIdAsync(int id);
        Task<IEnumerable<ClaimDocument>> FindAsync(Expression<Func<ClaimDocument, bool>> predicate);
        Task AddAsync(ClaimDocument entity);
        Task UpdateAsync(ClaimDocument entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimAsync(int claimId);

        Task<ClaimDocument?> GetByDocumentNameAsync(string documentName);
    }
}