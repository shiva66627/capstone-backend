using CapStone.Models;
using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.ClaimDocument
{
    public class ClaimDocumentRequestDTO
    {
        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Field { get; set; } = string.Empty;

        [Required]
        public DocumentReference DocumentReference { get; set; }

        [Required]
        public int ClaimId { get; set; }
    }
}