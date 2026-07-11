using CapStone.Models;

namespace CapStone.DTO.ClaimDocument
{
    public class ClaimDocumentResponseDTO
    {
        public int DocumentId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string Field { get; set; } = string.Empty;

        public DocumentReference DocumentReference { get; set; }

        public int ClaimId { get; set; }

        public DateTime UploadedDate { get; set; }
    }
}