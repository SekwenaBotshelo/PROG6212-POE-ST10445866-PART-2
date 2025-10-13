using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using System.Collections.Generic;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        // Shared claims from LecturerController
        private static List<Claim> ClaimsList = LecturerController.ClaimsList;

        // Dashboard with summary cards
        public IActionResult Dashboard()
        {
            ViewBag.PendingCount = ClaimsList.Count(c => c.Status == "Pending Verification");
            ViewBag.VerifiedCount = ClaimsList.Count(c => c.Status == "Verified");
            ViewBag.ApprovedCount = ClaimsList.Count(c => c.Status == "Approved");
            ViewBag.RejectedCount = ClaimsList.Count(c => c.Status == "Rejected");
            ViewBag.TotalCount = ClaimsList.Count;

            return View();
        }

        // Show verified claims for manager approval
        public IActionResult ApproveClaims()
        {
            var verifiedClaims = ClaimsList.Where(c => c.Status == "Verified").ToList();
            return View(verifiedClaims);
        }

        // Show details for manager approval
        public IActionResult ViewClaimDetails(int id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            // Explicitly specify view since it differs
            return View("ApproveClaimDetails", claim);
        }

        // POST: Approve a claim
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = "Approved"; // Manager approves
            }
            return RedirectToAction("ApproveClaims");
        }

        // POST: Reject a claim
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }
            return RedirectToAction("ApproveClaims");
        }

        // Reports page
        public IActionResult Reports()
        {
            // Pass all claims to the view (can later filter by month or status)
            var allClaims = ClaimsList;
            return View(allClaims);
        }
    }
}
