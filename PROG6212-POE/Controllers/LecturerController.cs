using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard shows all claims with documents
        public IActionResult Dashboard()
        {
            var claims = _context.Claims
                                 .Include(c => c.SupportingDocuments)
                                 .ToList();
            return View(claims);
        }

        // GET: Submit a new claim
        public IActionResult SubmitClaim()
        {
            return View(new Claim());
        }

        // POST: Submit claim
        [HttpPost]
        public IActionResult SubmitClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.Status = ClaimStatus.Pending;
                _context.Claims.Add(claim);
                _context.SaveChanges();
                // implementing Audit Trati
                _context.AuditTrails.Add(new AuditTrail
                {
                    Action = $"Claim submitted for {claim.LecturerName} ({claim.Month})",
                    Timestamp = DateTime.Now,
                    UserName = claim.LecturerName
                });
                _context.SaveChanges();


                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(claim);
        }

        // View submitted claims (Track Status)
        public IActionResult TrackClaim()
        {
            var claims = _context.Claims
                                 .Include(c => c.SupportingDocuments)
                                 .ToList();
            return View("TrackStatus", claims);
        }

        // GET: Upload document for a claim
        public IActionResult UploadDocuments(int? claimId)
        {
            if (claimId == null) return RedirectToAction("Dashboard");

            var claim = _context.Claims
                                .Include(c => c.SupportingDocuments)
                                .FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null) return RedirectToAction("Dashboard");

            return View("UploadDocument", claim);
        }

        // POST: Upload supporting document
        [HttpPost]
        public IActionResult UploadDocuments(int claimId, IFormFile supportingFile)
        {
            var claim = _context.Claims
                                .Include(c => c.SupportingDocuments)
                                .FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null) return RedirectToAction("Dashboard");

            if (supportingFile != null && supportingFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(supportingFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Only .pdf, .docx, and .xlsx files are allowed.";
                    return RedirectToAction("Dashboard");
                }

                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var uniqueFileName = $"{claimId}_{Path.GetFileName(supportingFile.FileName)}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    supportingFile.CopyTo(stream);
                }

                // Save document to database
                var doc = new Document
                {
                    FileName = supportingFile.FileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    ClaimId = claim.ClaimId,
                    UploadedOn = DateTime.Now
                };

                _context.Documents.Add(doc);
                _context.SaveChanges();
                _context.AuditTrails.Add(new AuditTrail
                {
                    Action = $"Document '{supportingFile.FileName}' uploaded for {claim.LecturerName}",
                    Timestamp = DateTime.Now,
                    UserName = claim.LecturerName
                });
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"File '{supportingFile.FileName}' uploaded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: Lecturer/SelectClaimForUpload
        public IActionResult SelectClaimForUpload()
        {
            // Fetch all claims
            var lecturerClaims = _context.Claims
                                         .Include(c => c.SupportingDocuments)
                                         .ToList();

            if (!lecturerClaims.Any())
            {
                TempData["ErrorMessage"] = "No claims available for uploading documents.";
            }

            return View(lecturerClaims);
        }
    }
}