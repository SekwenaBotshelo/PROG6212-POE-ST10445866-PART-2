using Xunit;
using PROG6212_POE.Controllers;
using PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;

namespace PROG6212_POE.Tests
{
    [Collection("NoParallelTests")]
    public class CoordinatorControllerTests
    {
        private CoordinatorController _controller;

        public CoordinatorControllerTests()
        {
            ResetTestData();
            _controller = new CoordinatorController();
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        private void ResetTestData()
        {
            lock (LecturerController.ClaimsList)
            {
                LecturerController.ClaimsList.Clear();
                // Use Coordinator-specific ID range: 301-399
                LecturerController.ClaimsList.Add(new Claim
                {
                    ClaimId = 301,
                    LecturerName = "Coordinator_John",
                    Month = "October",
                    TotalHours = 10,
                    HourlyRate = 150,
                    Status = "Pending Verification"
                });

                LecturerController.ClaimsList.Add(new Claim
                {
                    ClaimId = 302,
                    LecturerName = "Coordinator_Jane",
                    Month = "November",
                    TotalHours = 12,
                    HourlyRate = 120,
                    Status = "Pending Verification"
                });
            }
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            // Arrange
            ResetTestData();

            // Act
            var result = _controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void VerifyClaims_ReturnsOnlyPendingClaims()
        {
            // Arrange
            ResetTestData();

            // Act
            var result = _controller.VerifyClaims() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.All(model, c => Assert.Equal("Pending Verification", c.Status));
        }

        [Fact]
        public void Verify_ValidClaim_UpdatesStatusToVerified()
        {
            // Arrange
            ResetTestData();

            // Act
            _controller.Verify(301);

            // Assert
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == 301);
            Assert.Equal("Verified", claim.Status);
        }

        [Fact]
        public void Reject_ValidClaim_UpdatesStatusToRejected()
        {
            // Arrange
            ResetTestData();

            // Act
            _controller.Reject(302);

            // Assert
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == 302);
            Assert.Equal("Rejected", claim.Status);
        }

        [Fact]
        public void ViewClaimDetails_ExistingClaim_ReturnsViewWithClaim()
        {
            // Arrange
            ResetTestData();

            // Act
            var result = _controller.ViewClaimDetails(301) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Claim>(result.Model);
            Assert.Equal(301, model.ClaimId);
        }

        [Fact]
        public void ViewClaimDetails_NonExistingClaim_ReturnsNotFound()
        {
            // Arrange
            ResetTestData();

            // Act
            var result = _controller.ViewClaimDetails(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}