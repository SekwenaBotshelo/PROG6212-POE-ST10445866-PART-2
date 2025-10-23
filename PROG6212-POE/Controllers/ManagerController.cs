using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AppDbContext _context;

        public ManagerController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard with summary cards
        public IActionResult Dashboard()
        {
            var claims = _context.Claims.ToList();

            ViewBag.PendingCount = claims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.VerifiedCount = claims.Count(c => c.Status == ClaimStatus.Verified);
            ViewBag.ApprovedCount = claims.Count(c => c.Status == ClaimStatus.Approved);
            ViewBag.RejectedCount = claims.Count(c => c.Status == ClaimStatus.Rejected);
            ViewBag.TotalCount = claims.Count;

            return View(claims);
        }

        // Show verified claims for manager approval
        public IActionResult ApproveClaims()
        {
            var verifiedClaims = _context.Claims
                                         .Where(c => c.Status == ClaimStatus.Verified)
                                         .ToList();
            return View(verifiedClaims);
        }

        // Show claim details for approval
        public IActionResult ViewClaimDetails(int id)
        {
            var claim = _context.Claims
                                .Include(c => c.SupportingDocuments)
                                .FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            return View("ApproveClaimDetails", claim);
        }

        // POST: Approve a claim (automated rules)
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                // Automated approval rule (example: TotalAmount < 5000 auto-approve)
                if (claim.TotalAmount <= 5000)
                    claim.Status = ClaimStatus.Approved;
                else
                    claim.Status = ClaimStatus.Verified; // Keep for manual review if needed

                _context.SaveChanges();

                // Audit Trail logging
                LogAudit($"Claim {(claim.Status == ClaimStatus.Approved ? "approved" : "kept for review")} for {claim.LecturerName} (Claim ID: {claim.ClaimId})", "Manager");
            }

            return RedirectToAction("ApproveClaims");
        }

        // POST: Reject a claim
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Rejected;
                _context.SaveChanges();

                // Audit Trail logging
                LogAudit($"Claim rejected for {claim.LecturerName} (Claim ID: {claim.ClaimId})", "Manager");
            }

            return RedirectToAction("ApproveClaims");
        }

        // Reports page
        public IActionResult Reports()
        {
            var allClaims = _context.Claims
                                    .Include(c => c.SupportingDocuments)
                                    .ToList();

            // Log report generation
            LogAudit("Manager generated claims report", "Manager");

            return View(allClaims);
        }

        // Private helper for audit logging
        private void LogAudit(string action, string userName)
        {
            _context.AuditTrails.Add(new AuditTrail
            {
                Action = action,
                Timestamp = DateTime.Now,
                UserName = userName
            });
            _context.SaveChanges();
        }
    }
}
