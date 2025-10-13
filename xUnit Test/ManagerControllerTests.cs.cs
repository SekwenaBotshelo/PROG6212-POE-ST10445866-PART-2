using PROG6212_POE.Controllers;
using PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace PROG6212_POE.Tests
{
    public class ManagerControllerTests
    {
        public ManagerControllerTests()
        {
            // Ensure the shared list is cleared before each test
            LecturerController.ClaimsList.Clear();

            // Seed with test data
            LecturerController.ClaimsList.Add(new Claim
            {
                ClaimId = 101,
                LecturerName = "Alice",
                TotalHours = 10,
                HourlyRate = 150,
                Month = "October",
                Status = "Verified"
            });

            LecturerController.ClaimsList.Add(new Claim
            {
                ClaimId = 102,
                LecturerName = "Bob",
                TotalHours = 8,
                HourlyRate = 200,
                Month = "October",
                Status = "Verified"
            });
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims_WithCorrectCounts()
        {
            // Arrange
            var controller = new ManagerController();

            // Act
            var result = controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);

            // Check summary counts
            Assert.Equal(0, controller.ViewBag.PendingCount);
            Assert.Equal(2, controller.ViewBag.VerifiedCount);
            Assert.Equal(0, controller.ViewBag.ApprovedCount);
            Assert.Equal(0, controller.ViewBag.RejectedCount);
            Assert.Equal(2, controller.ViewBag.TotalCount);
        }

        [Fact]
        public void ApproveClaims_ReturnsOnlyVerifiedClaims()
        {
            // Arrange
            var controller = new ManagerController();

            // Act
            var result = controller.ApproveClaims() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.All(model, c => Assert.Equal("Verified", c.Status));
        }

        [Fact]
        public void ViewClaimDetails_ValidId_ReturnsClaim()
        {
            var controller = new ManagerController();
            int testId = 101;

            var result = controller.ViewClaimDetails(testId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<Claim>(result.Model);
            Assert.Equal(testId, model.ClaimId);
        }

        [Fact]
        public void Approve_ValidId_UpdatesStatus()
        {
            var controller = new ManagerController();
            int testId = 101;

            var result = controller.Approve(testId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);

            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == testId);
            Assert.Equal("Approved", claim.Status);
        }

        [Fact]
        public void Reject_ValidId_UpdatesStatus()
        {
            var controller = new ManagerController();
            int testId = 102;

            var result = controller.Reject(testId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);

            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == testId);
            Assert.Equal("Rejected", claim.Status);
        }

        [Fact]
        public void Reports_ReturnsAllClaims()
        {
            var controller = new ManagerController();

            var result = controller.Reports() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count); // Should match seeded claims
        }
    }
}
