using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Data;
using System.Linq;

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
            var claims = _context.Claims.ToList();

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
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            return View("VerifyClaimDetails", claim);
        }

        // POST: Verify a claim
        [HttpPost]
        public IActionResult Verify(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Verified;
                _context.SaveChanges();
            }
            return RedirectToAction("VerifyClaims");
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
            }
            return RedirectToAction("VerifyClaims");
        }

        // Reports page
        public IActionResult Reports()
        {
            var allClaims = _context.Claims.ToList();
            return View(allClaims);
        }
    }
}