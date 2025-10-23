using System;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models
{
    public class AuditTrail
    {
        [Key]
        public int AuditTrailId { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}