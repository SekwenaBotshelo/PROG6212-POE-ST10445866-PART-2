using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Linq;

public static class DemoDataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Claims.Any())
        {
            var claim1 = new Claim
            {
                LecturerName = "John Smith",
                Month = "October",
                TotalHours = 20,
                HourlyRate = 200,
                Status = ClaimStatus.Pending
            };

            var claim2 = new Claim
            {
                LecturerName = "Jane Doe",
                Month = "October",
                TotalHours = 15,
                HourlyRate = 250,
                Status = ClaimStatus.Verified
            };

            context.Claims.AddRange(claim1, claim2);
            context.SaveChanges();

            claim1.SupportingDocuments.Add(new Document
            {
                FileName = "Timesheet_John.pdf",
                FilePath = "/uploads/Timesheet_John.pdf",
                ClaimId = claim1.ClaimId
            });

            claim2.SupportingDocuments.Add(new Document
            {
                FileName = "Invoice_Jane.xlsx",
                FilePath = "/uploads/Invoice_Jane.xlsx",
                ClaimId = claim2.ClaimId
            });

            context.SaveChanges();
        }

        // Seed Coordinators
        if (!context.Coordinators.Any())
        {
            context.Coordinators.Add(new Coordinator
            {
                Name = "Mark Daniels",
                Email = "mark.daniels@campus.edu"
            });
            context.SaveChanges();
        }

        // Seed Managers
        if (!context.Managers.Any())
        {
            context.Managers.Add(new Manager
            {
                Name = "Linda Khoza",
                Email = "linda.khoza@campus.edu"
            });
            context.SaveChanges();
        }

        // Seed HR
        if (!context.HRs.Any())
        {
            context.HRs.Add(new HR
            {
                Name = "Zanele Mthembu",
                Email = "zanele.hr@campus.edu"
            });
            context.SaveChanges();
        }

        // Seed a few Invoices for HR automation demo
        if (!context.Invoices.Any())
        {
            context.Invoices.Add(new Invoice
            {
                ClaimId = 1,
                LecturerName = "John Smith",
                AmountPaid = 4000,
                PaymentStatus = "Paid"
            });

            context.Invoices.Add(new Invoice
            {
                ClaimId = 2,
                LecturerName = "Jane Doe",
                AmountPaid = 3750,
                PaymentStatus = "Processing"
            });

            context.SaveChanges();
        }
    }
}