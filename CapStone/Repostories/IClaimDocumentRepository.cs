using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IClaimDocumentRepository : IGenericRepository<ClaimDocument>
    {
        Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimAsync(int claimId);

        Task<ClaimDocument?> GetByDocumentNameAsync(string documentName);
    }
}