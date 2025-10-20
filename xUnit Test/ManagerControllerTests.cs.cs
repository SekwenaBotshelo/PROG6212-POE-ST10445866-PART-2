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
    public class ManagerControllerTests
    {
        private ManagerController GetController(AppDbContext context)
        {
            return new ManagerController(context);
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
                    ClaimId = 201,
                    LecturerName = "Manager_Alice",
                    Month = "October",
                    TotalHours = 10,
                    HourlyRate = 150,
                    Status = ClaimStatus.Verified
                },
                new Claim
                {
                    ClaimId = 202,
                    LecturerName = "Manager_Bob",
                    Month = "October",
                    TotalHours = 8,
                    HourlyRate = 200,
                    Status = ClaimStatus.Verified
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
        public void Approve_ValidId_UpdatesStatus()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.Approve(201) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);

            var claim = context.Claims.First(c => c.ClaimId == 201);
            Assert.Equal(ClaimStatus.Approved, claim.Status);
        }

        [Fact]
        public void Reject_ValidId_UpdatesStatus()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.Reject(202) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);

            var claim = context.Claims.First(c => c.ClaimId == 202);
            Assert.Equal(ClaimStatus.Rejected, claim.Status);
        }

        [Fact]
        public void Reports_ReturnsAllClaims()
        {
            var context = GetNewInMemoryDb();
            SeedClaims(context);
            var controller = GetController(context);

            var result = controller.Reports() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);
        }
    }
}
