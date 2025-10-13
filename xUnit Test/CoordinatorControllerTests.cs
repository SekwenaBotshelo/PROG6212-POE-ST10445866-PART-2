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
    public class CoordinatorControllerTests
    {
        private CoordinatorController _controller;

        public CoordinatorControllerTests()
        {
            // Reset static ClaimsList before each test
            LecturerController.ClaimsList.Clear();
            LecturerController.ClaimsList.Add(new Claim
            {
                ClaimId = 101,
                LecturerName = "John Doe",
                Month = "October",
                TotalHours = 10,
                HourlyRate = 150
            });
            LecturerController.ClaimsList.Add(new Claim
            {
                ClaimId = 102,
                LecturerName = "Jane Doe",
                Month = "November",
                TotalHours = 12,
                HourlyRate = 120
            });

            _controller = new CoordinatorController();

            // Mock TempData
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            var result = _controller.Dashboard() as ViewResult;
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void VerifyClaims_ReturnsOnlyPendingClaims()
        {
            // Both claims are initially Pending Verification
            var result = _controller.VerifyClaims() as ViewResult;
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.All(model, c => Assert.Equal("Pending Verification", c.Status));
        }

        [Fact]
        public void Verify_ValidClaim_UpdatesStatusToVerified()
        {
            _controller.Verify(101);
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == 101);
            Assert.Equal("Verified", claim.Status);
        }

        [Fact]
        public void Reject_ValidClaim_UpdatesStatusToRejected()
        {
            _controller.Reject(102);
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == 102);
            Assert.Equal("Rejected", claim.Status);
        }

        [Fact]
        public void ViewClaimDetails_ExistingClaim_ReturnsViewWithClaim()
        {
            var result = _controller.ViewClaimDetails(101) as ViewResult;
            var model = Assert.IsType<Claim>(result.Model);
            Assert.Equal(101, model.ClaimId);
        }

        [Fact]
        public void ViewClaimDetails_NonExistingClaim_ReturnsNotFound()
        {
            var result = _controller.ViewClaimDetails(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}