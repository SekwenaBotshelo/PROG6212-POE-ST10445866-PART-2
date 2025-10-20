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

        // Foreign key relationship
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public Claim? Claim { get; set; }
    }
}