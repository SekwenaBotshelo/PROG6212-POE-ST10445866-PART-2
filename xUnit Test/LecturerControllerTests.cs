using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PROG6212_POE.Controllers;
using PROG6212_POE.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PROG6212_POE.Tests
{
    [Collection("NoParallelTests")]
    public class LecturerControllerTests
    {
        private LecturerController GetControllerWithTempData()
        {
            var controller = new LecturerController();
            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;
            return controller;
        }

        private void ResetTestData()
        {
            lock (LecturerController.ClaimsList)
            {
                LecturerController.ClaimsList.Clear();
                // Use Lecturer-specific ID range: 101-199
                LecturerController.ClaimsList.Add(new Claim
                {
                    ClaimId = 101,
                    LecturerName = "Lecturer_Alice",
                    TotalHours = 10,
                    HourlyRate = 150,
                    Month = "October",
                    Status = "Pending Verification"
                });
            }
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            // Arrange
            ResetTestData();
            var controller = GetControllerWithTempData();

            // Act
            var result = controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public void SubmitClaim_ValidClaim_AddsClaim()
        {
            // Arrange
            ResetTestData();
            var controller = GetControllerWithTempData();
            var claim = new Claim
            {
                LecturerName = "Lecturer_Bob",
                TotalHours = 8,
                HourlyRate = 200,
                Month = "October"
            };

            // Act
            var result = controller.SubmitClaim(claim) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
            Assert.Equal(2, LecturerController.ClaimsList.Count); // Original + new claim
            var submittedClaim = LecturerController.ClaimsList.Last();
            Assert.Equal("Pending Verification", submittedClaim.Status);
        }

        [Fact]
        public void UploadDocuments_NullClaimId_RedirectsToDashboard()
        {
            // Arrange
            var controller = GetControllerWithTempData();

            // Act
            var result = controller.UploadDocuments(null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
        }

        [Fact]
        public void UploadDocuments_InvalidFileType_ReturnsError()
        {
            // Arrange
            ResetTestData();
            var controller = GetControllerWithTempData();

            var content = "dummy content";
            var fileName = "test.txt"; // Invalid file type
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "supportingFile", fileName);

            // Act
            var result = controller.UploadDocuments(101, file) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UploadDocument", result.ViewName);
            var model = Assert.IsType<Claim>(result.Model);
            Assert.Equal(101, model.ClaimId);
        }

        [Fact]
        public void UploadDocuments_ValidFile_AddsFileToClaim()
        {
            // Arrange
            ResetTestData();
            var controller = GetControllerWithTempData();

            var content = "dummy content";
            var fileName = "file.pdf";
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "supportingFile", fileName);

            // Act
            var result = controller.UploadDocuments(101, file) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UploadDocument", result.ViewName);
            var claim = LecturerController.ClaimsList.First(c => c.ClaimId == 101);
            Assert.Contains($"{claim.ClaimId}_{fileName}", claim.UploadedFiles);
        }
    }
}