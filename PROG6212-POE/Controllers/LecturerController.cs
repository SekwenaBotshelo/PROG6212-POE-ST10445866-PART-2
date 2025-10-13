using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private static List<Claim> _claims = new List<Claim>();
        private static int _nextClaimId = 101; // starting ClaimId

        // Dashboard shows all claims
        public IActionResult Dashboard()
        {
            return View(_claims);
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
                _claims.Add(claim);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(claim);
        }

        // View Submitted Claims
        public IActionResult TrackClaim()
        {
            return View("TrackStatus", _claims);
        }

        // GET: Upload Supporting Documents
        public IActionResult UploadDocuments()
        {
            return View("UploadDocument");
        }

        // POST: Upload Supporting Documents
        [HttpPost]
        public IActionResult UploadDocuments(IFormFile supportingFile)
        {
            if (supportingFile != null && supportingFile.Length > 0)
            {
                // Only allow txt files
                var allowedExtensions = new[] { ".txt" };
                var extension = Path.GetExtension(supportingFile.FileName);

                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    TempData["ErrorMessage"] = "Only .txt files are allowed.";
                    return View("UploadDocument");
                }

                // Save to wwwroot/uploads (make sure this folder exists)
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, supportingFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    supportingFile.CopyTo(stream);
                }

                TempData["SuccessMessage"] = $"File '{supportingFile.FileName}' uploaded successfully!";
                return View("UploadDocument");
            }

            TempData["ErrorMessage"] = "Please select a file to upload.";
            return View("UploadDocument");
        }
    }
}
