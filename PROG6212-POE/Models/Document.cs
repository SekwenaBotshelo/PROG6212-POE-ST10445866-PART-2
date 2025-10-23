using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        // Timestamp for upload
        [Required]
        public DateTime UploadedOn { get; set; } = DateTime.Now;

        // Foreign key relationship
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public Claim? Claim { get; set; }
    }
}