using CapStone.DTO.ClaimDocument;
using CapStone.DTO.Common;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class ClaimDocumentService : IClaimDocumentService
    {
        private readonly IClaimDocumentRepository _documentRepository;
        private readonly IClaimRepository _claimRepository;

        public ClaimDocumentService(
            IClaimDocumentRepository documentRepository,
            IClaimRepository claimRepository)
        {
            _documentRepository = documentRepository;
            _claimRepository = claimRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>> GetDocumentsByClaimAsync(int claimId)
        {
            try
            {
                var documents = await _documentRepository.GetDocumentsByClaimAsync(claimId);

                var result = documents.Select(d => new ClaimDocumentResponseDTO
                {
                    DocumentId = d.DocumentId,
                    DocumentName = d.DocumentName,
                    DocumentType = d.DocumentType,
                    Field = d.Field,
                    DocumentReference = d.DocumentReference,
                    ClaimId = d.ClaimId,
                    CustomerName = d.Claim?.Policy?.Customer?.User?.UserName ?? "N/A",
                });

                return new ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>
                {
                    Success = true,
                    Message = "Documents retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();

                var result = documents.Select(d => new ClaimDocumentResponseDTO
                {
                    DocumentId = d.DocumentId,
                    DocumentName = d.DocumentName,
                    DocumentType = d.DocumentType,
                    Field = d.Field,
                    DocumentReference = d.DocumentReference,
                    ClaimId = d.ClaimId,
                    CustomerName = d.Claim?.Policy?.Customer?.User?.UserName ?? "N/A",
                    
                });

                return new ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>
                {
                    Success = true,
                    Message = "Documents retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<ClaimDocumentResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> UploadDocumentAsync(ClaimDocumentRequestDTO request)
        {
            try
            {
                var claim = await _claimRepository.GetByIdAsync(request.ClaimId);

                if (claim == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Claim not found."
                    };
                }

                var document = new ClaimDocument
                {
                    DocumentName = request.DocumentName,
                    DocumentType = request.DocumentType,
                    Field = request.Field,
                    DocumentReference = request.DocumentReference,
                    ClaimId = request.ClaimId,
                };

                await _documentRepository.AddAsync(document);
                await _documentRepository.SaveChangesAsync();

                // If claim was flagged by staff for rejection or re-upload, update status to notify staff
                if (!string.IsNullOrWhiteSpace(claim.AgentRemarks) &&
                    (claim.AgentRemarks.Contains("reject", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("re-upload", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("reupload", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("proof", StringComparison.OrdinalIgnoreCase) ||
                     claim.AgentRemarks.Contains("missing", StringComparison.OrdinalIgnoreCase)))
                {
                    claim.AgentRemarks = $"[Re-uploaded on {DateTime.UtcNow:dd-MMM-yyyy}] Customer provided updated documents. Ready for Staff re-evaluation.";
                    claim.UpdatedDate = DateTime.UtcNow;
                    await _claimRepository.UpdateAsync(claim);
                    await _claimRepository.SaveChangesAsync();
                }

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Document uploaded successfully. Claim marked for staff re-evaluation."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}