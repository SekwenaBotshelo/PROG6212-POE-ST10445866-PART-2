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
    public class LecturerControllerTests
    {
        private LecturerController GetControllerWithTempData()
        {
            var controller = new LecturerController();

            // Mock TempData
            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;

            return controller;
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            // Arrange
            var controller = GetControllerWithTempData();
            LecturerController.ClaimsList.Clear();
            LecturerController.ClaimsList.Add(new Claim { ClaimId = 101, LecturerName = "Alice", TotalHours = 10, HourlyRate = 150, Month = "October" });

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
            var controller = GetControllerWithTempData();
            LecturerController.ClaimsList.Clear();
            var claim = new Claim { LecturerName = "Bob", TotalHours = 8, HourlyRate = 200, Month = "October" };

            // Act
            var result = controller.SubmitClaim(claim) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
            Assert.Single(LecturerController.ClaimsList);
            Assert.Equal("Pending Verification", LecturerController.ClaimsList[0].Status);
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
            var controller = GetControllerWithTempData();
            LecturerController.ClaimsList.Clear();
            var claim = new Claim { ClaimId = 101, LecturerName = "Alice", TotalHours = 10, HourlyRate = 150, Month = "October" };
            LecturerController.ClaimsList.Add(claim);

            var content = "dummy content";
            var fileName = "test.txt"; // Invalid for this controller (.pdf, .docx, .xlsx allowed)
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "supportingFile", fileName);

            // Act
            var result = controller.UploadDocuments(101, file) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UploadDocument", result.ViewName);
            Assert.Equal(claim, result.Model);
        }

        [Fact]
        public void UploadDocuments_ValidFile_AddsFileToClaim()
        {
            // Arrange
            var controller = GetControllerWithTempData();
            LecturerController.ClaimsList.Clear();
            var claim = new Claim { ClaimId = 101, LecturerName = "Alice", TotalHours = 10, HourlyRate = 150, Month = "October" };
            LecturerController.ClaimsList.Add(claim);

            var content = "dummy content";
            var fileName = "file.pdf";
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "supportingFile", fileName);

            // Act
            var result = controller.UploadDocuments(101, file) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UploadDocument", result.ViewName);
            Assert.Contains($"{claim.ClaimId}_{fileName}", claim.UploadedFiles);
        }
    }
}
