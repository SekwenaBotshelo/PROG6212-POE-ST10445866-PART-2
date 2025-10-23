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

        // Dashboard: Display summary cards and all claims
        public IActionResult Dashboard()
        {
            var claims = _context.Claims.ToList();
            PopulateClaimCounts(claims); // Set ViewBag counts for summary cards
            return View(claims);
        }

        // Approve Claims: Display claims verified by coordinator for manager approval
        public IActionResult ApproveClaims()
        {
            var verifiedClaims = _context.Claims
                                         .Where(c => c.Status == ClaimStatus.Verified)
                                         .Include(c => c.SupportingDocuments)
                                         .ToList();
            return View(verifiedClaims);
        }

        // View Claim Details: Display full details of a specific claim including supporting documents
        public IActionResult ViewClaimDetails(int id)
        {
            var claim = _context.Claims
                                .Include(c => c.SupportingDocuments)
                                .FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            return View("ApproveClaimDetails", claim);
        }

        // POST: Approve a claim
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                // Automated approval rule: Auto-approve if TotalAmount <= 5000
                if (claim.TotalAmount <= 5000)
                    claim.Status = ClaimStatus.Approved;
                else
                    claim.Status = ClaimStatus.Verified; // Keep for manual review if needed

                _context.SaveChanges();

                // Log action to AuditTrail
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

                // Log action to AuditTrail
                LogAudit($"Claim rejected for {claim.LecturerName} (Claim ID: {claim.ClaimId})", "Manager");
            }

            return RedirectToAction("ApproveClaims");
        }

        // Reports: Display all claims with supporting documents for reporting purposes
        public IActionResult Reports()
        {
            var allClaims = _context.Claims
                                    .Include(c => c.SupportingDocuments)
                                    .ToList();

            // Log report generation
            LogAudit("Manager generated claims report", "Manager");

            return View(allClaims);
        }

        // -------------------- PRIVATE HELPERS --------------------

        // Helper method to populate ViewBag with claim counts for dashboard summary cards
        private void PopulateClaimCounts(System.Collections.Generic.List<Claim> claims)
        {
            ViewBag.PendingCount = claims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.VerifiedCount = claims.Count(c => c.Status == ClaimStatus.Verified);
            ViewBag.ApprovedCount = claims.Count(c => c.Status == ClaimStatus.Approved);
            ViewBag.RejectedCount = claims.Count(c => c.Status == ClaimStatus.Rejected);
            ViewBag.TotalCount = claims.Count;
        }

        // Helper method to log audit actions
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
