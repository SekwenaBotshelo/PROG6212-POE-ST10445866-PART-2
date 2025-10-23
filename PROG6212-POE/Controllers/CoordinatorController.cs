using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly AppDbContext _context;

        public CoordinatorController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard with summary cards
        public IActionResult Dashboard()
        {
            var claims = _context.Claims
                                 .Include(c => c.SupportingDocuments)
                                 .ToList();

            ViewBag.PendingCount = claims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.VerifiedCount = claims.Count(c => c.Status == ClaimStatus.Verified);
            ViewBag.TotalCount = claims.Count;

            return View(claims);
        }

        // Show pending claims for verification
        public IActionResult VerifyClaims()
        {
            var pendingClaims = _context.Claims
                                        .Where(c => c.Status == ClaimStatus.Pending)
                                        .ToList();
            return View(pendingClaims);
        }

        // Show claim details for verification
        public IActionResult ViewClaimDetails(int id)
        {
            var claim = _context.Claims
                                .Include(c => c.SupportingDocuments)
                                .FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            return View("VerifyClaimDetails", claim);
        }

        // POST: Verify a claim (automated)
        [HttpPost]
        public IActionResult Verify(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                // Automated verification rules
                if (claim.TotalHours <= 200 && claim.HourlyRate <= 1000)
                    claim.Status = ClaimStatus.Verified;
                else
                    claim.Status = ClaimStatus.Rejected;

                _context.SaveChanges();

                // Audit Trail logging
                _context.AuditTrails.Add(new AuditTrail
                {
                    Action = $"Claim {(claim.Status == ClaimStatus.Verified ? "verified" : "rejected")} for {claim.LecturerName} (Claim ID: {claim.ClaimId})",
                    Timestamp = DateTime.Now,
                    UserName = "Coordinator" // Replace with logged-in coordinator username when auth is implemented
                });
                _context.SaveChanges();
            }

            return RedirectToAction("VerifyClaims");
        }

        // POST: Reject a claim manually
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Rejected;
                _context.SaveChanges();

                // Audit Trail logging
                _context.AuditTrails.Add(new AuditTrail
                {
                    Action = $"Claim rejected manually for {claim.LecturerName} (Claim ID: {claim.ClaimId})",
                    Timestamp = DateTime.Now,
                    UserName = "Coordinator" // Replace with logged-in coordinator username when auth is implemented
                });
                _context.SaveChanges();
            }

            return RedirectToAction("VerifyClaims");
        }

        // Reports page
        public IActionResult Reports()
        {
            var allClaims = _context.Claims
                                    .Include(c => c.SupportingDocuments)
                                    .ToList();

            // Optional: log that a report was generated
            _context.AuditTrails.Add(new AuditTrail
            {
                Action = $"Coordinator generated claims report",
                Timestamp = DateTime.Now,
                UserName = "Coordinator"
            });
            _context.SaveChanges();

            return View(allClaims);
        }
    }
}
