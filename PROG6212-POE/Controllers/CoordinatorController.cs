using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using System.Collections.Generic;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        // Shared claims from LecturerController
        private static List<Claim> ClaimsList = LecturerController.ClaimsList;

        // Dashboard with summary cards
        public IActionResult Dashboard()
        {
            ViewBag.PendingCount = ClaimsList.Count(c => c.Status == "Pending Verification");
            ViewBag.VerifiedCount = ClaimsList.Count(c => c.Status == "Verified");
            ViewBag.TotalCount = ClaimsList.Count;

            return View();
        }

        // Show pending claims for verification
        public IActionResult VerifyClaims()
        {
            var pendingClaims = ClaimsList.Where(c => c.Status == "Pending Verification").ToList();
            return View(pendingClaims);
        }

        // Show claim details for verification
        public IActionResult ViewClaimDetails(int id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            // Explicitly specify the view name since it differs
            return View("VerifyClaimDetails", claim);
        }

        // POST: Verify a claim
        [HttpPost]
        public IActionResult Verify(int id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = "Verified"; // Coordinator approves
            }
            return RedirectToAction("VerifyClaims");
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
            return RedirectToAction("VerifyClaims");
        }
    }
}
