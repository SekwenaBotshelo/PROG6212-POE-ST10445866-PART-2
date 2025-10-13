using PROG6212_POE.Controllers;
using PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PROG6212_POE.Tests
{
    [Collection("NoParallelTests")]
    public class ManagerControllerTests
    {
        public ManagerControllerTests()
        {
            ResetTestData();
        }

        private void ResetTestData()
        {
            lock (LecturerController.ClaimsList)
            {
                LecturerController.ClaimsList.Clear();
                // Use Manager-specific ID range: 201-299
                LecturerController.ClaimsList.Add(new Claim
                {
                    ClaimId = 201,
                    LecturerName = "Manager_Alice",
                    TotalHours = 10,
                    HourlyRate = 150,
                    Month = "October",
                    Status = "Verified"
                });

                LecturerController.ClaimsList.Add(new Claim
                {
                    ClaimId = 202,
                    LecturerName = "Manager_Bob",
                    TotalHours = 8,
                    HourlyRate = 200,
                    Month = "October",
                    Status = "Verified"
                });
            }
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims_WithCorrectCounts()
        {
            // Arrange
            ResetTestData();
            var controller = new ManagerController();

            // Verify setup
            Assert.Equal(2, LecturerController.ClaimsList.Count);

            // Act
            var result = controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);

            // Remove ViewBag checks if not implemented, or implement them in controller
            // If ViewBag is used, make sure your controller sets these values
        }

        [Fact]
        public void ApproveClaims_ReturnsOnlyVerifiedClaims()
        {
            // Arrange
            ResetTestData();
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
            // Arrange
            ResetTestData();
            var controller = new ManagerController();
            int testId = 201;

            // Act
            var result = controller.ViewClaimDetails(testId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<Claim>(result.Model);
            Assert.Equal(testId, model.ClaimId);
        }

        [Fact]
        public void Approve_ValidId_UpdatesStatus()
        {
            // Arrange
            ResetTestData();
            var controller = new ManagerController();
            int testId = 201;

            // Act
            var result = controller.Approve(testId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == testId);
            Assert.Equal("Approved", claim.Status);
        }

        [Fact]
        public void Reject_ValidId_UpdatesStatus()
        {
            // Arrange
            ResetTestData();
            var controller = new ManagerController();
            int testId = 202;

            // Act
            var result = controller.Reject(testId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApproveClaims", result.ActionName);
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == testId);
            Assert.Equal("Rejected", claim.Status);
        }

        [Fact]
        public void Reports_ReturnsAllClaims()
        {
            // Arrange
            ResetTestData();
            var controller = new ManagerController();

            // Verify setup
            Assert.Equal(2, LecturerController.ClaimsList.Count);

            // Act
            var result = controller.Reports() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);
        }
    }

    // Add this collection definition to prevent parallel execution
    [CollectionDefinition("NoParallelTests", DisableParallelization = true)]
    public class NoParallelTestsCollection
    {
    }
}