using PROG6212_POE.Data;
using PROG6212_POE.Models;

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
    }
}