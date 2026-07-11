using System.ComponentModel.DataAnnotations;

namespace CapStone.Models
{
    public enum DocumentReference
    {
        Url,
        ReferenceText,
        FileName
    }

    public class ClaimDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Field { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DocumentReference DocumentReference { get; set; }

        // Foreign Key
        public int ClaimId { get; set; }

        // Navigation Property
        public Claim? Claim { get; set; }
    }
}