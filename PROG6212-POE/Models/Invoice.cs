using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public Claim? Claim { get; set; }

        [Required]
        public string LecturerName { get; set; } = string.Empty;

        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string PaymentStatus { get; set; } = "Pending Payment";
    }
}