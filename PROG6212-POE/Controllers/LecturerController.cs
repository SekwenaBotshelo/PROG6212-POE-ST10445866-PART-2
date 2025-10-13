using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        // Shared claim list for all roles
        public static List<Claim> ClaimsList = new List<Claim>();
        private static int _nextClaimId = 101;

        // Dashboard shows all claims
        public IActionResult Dashboard()
        {
            return View(ClaimsList);
        }

        // GET: SubmitClaim form
        public IActionResult SubmitClaim()
        {
            return View(new Claim());
        }

        // POST: SubmitClaim form
        [HttpPost]
        public IActionResult SubmitClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.ClaimId = _nextClaimId++;
                claim.Status = "Pending Verification"; // default
                ClaimsList.Add(claim);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(claim);
        }

        // View Submitted Claims
        public IActionResult TrackClaim()
        {
            return View("TrackStatus", ClaimsList);
        }

        // GET: Upload Supporting Documents
        public IActionResult UploadDocuments(int? claimId)
        {
            if (claimId == null)
            {
                TempData["ErrorMessage"] = "Claim ID is required to upload a document.";
                return RedirectToAction("Dashboard");
            }

            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("Dashboard");
            }

            return View("UploadDocument", claim);
        }

        // POST: Upload Supporting Documents
        [HttpPost]
        public IActionResult UploadDocuments(int claimId, IFormFile supportingFile)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("Dashboard");
            }

            if (supportingFile != null && supportingFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(supportingFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Only .pdf, .docx, and .xlsx files are allowed.";
                    return View("UploadDocument", claim);
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

                claim.UploadedFiles.Add(uniqueFileName);
                TempData["SuccessMessage"] = $"File '{supportingFile.FileName}' uploaded successfully!";
                return View("UploadDocument", claim);
            }

            TempData["ErrorMessage"] = "Please select a file to upload.";
            return View("UploadDocument", claim);
        }
    }
}