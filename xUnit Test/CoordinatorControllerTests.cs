using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Controllers;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace PROG6212_POE.Tests
{
    public class CoordinatorControllerTests
    {
        private CoordinatorController GetController(AppDbContext context)
        {
            return new CoordinatorController(context);
        }

        private AppDbContext GetNewInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private void SeedClaims(AppDbContext context)
        {
            context.Claims.AddRange(
                new Claim
                {
                    ClaimId = 301,
                    LecturerName = "Coordinator_John",
                    Month = "October",
                    TotalHours = 10,
                    HourlyRate = 150,
                    Status = ClaimStatus.Pending
                },
                new Claim
                {
                    ClaimId = 302,
                    LecturerName = "Coordinator_Jane",
                    Month = "November",
                    TotalHours = 12,
                    HourlyRate = 120,
                    Status = ClaimStatus.Pending
                }
            );
            context.SaveChanges();
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.Dashboard() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Verify_ValidClaim_UpdatesStatusToVerified()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            controller.Verify(301);

            var claim = context.Claims.First(c => c.ClaimId == 301);
            Assert.Equal(ClaimStatus.Verified, claim.Status);
        }

        [Fact]
        public void Reject_ValidClaim_UpdatesStatusToRejected()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            controller.Reject(302);

            var claim = context.Claims.First(c => c.ClaimId == 302);
            Assert.Equal(ClaimStatus.Rejected, claim.Status);
        }

        [Fact]
        public void ViewClaimDetails_ExistingClaim_ReturnsViewWithClaim()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.ViewClaimDetails(301) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Claim>(result.Model);
            Assert.Equal(301, model.ClaimId);
        }

        [Fact]
        public void ViewClaimDetails_NonExistingClaim_ReturnsNotFound()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.ViewClaimDetails(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
