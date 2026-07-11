using CapStone.DTO.ClaimDocument;
using CapStone.DTO.Common;



namespace CapStone.Services
{
    public interface IClaimDocumentService
    {
        Task<ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>> GetDocumentsByClaimAsync(int claimId);

        Task<ApiResponseDTO<string>> UploadDocumentAsync(ClaimDocumentRequestDTO request);
    }
}