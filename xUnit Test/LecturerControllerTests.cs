using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PROG6212_POE.Controllers;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace PROG6212_POE.Tests
{
    public class LecturerControllerTests
    {
        private LecturerController GetController(AppDbContext context)
        {
            var controller = new LecturerController(context);
            controller.TempData = new Mock<ITempDataDictionary>().Object;
            return controller;
        }

        private AppDbContext GetNewInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Dashboard_ReturnsAllClaims()
        {
            // Arrange
            var context = GetNewInMemoryDb();
            context.Claims.Add(new Claim
            {
                ClaimId = 101,
                LecturerName = "Alice",
                Month = "October",
                TotalHours = 10,
                HourlyRate = 150,
                Status = ClaimStatus.Pending
            });
            context.SaveChanges();

            var controller = GetController(context);

            // Act
            var result = controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Single(model);
            Assert.Equal(ClaimStatus.Pending, model[0].Status);
        }

        [Fact]
        public void SubmitClaim_ValidClaim_AddsClaim()
        {
            // Arrange
            var context = GetNewInMemoryDb();
            var controller = GetController(context);

            var claim = new Claim
            {
                LecturerName = "Bob",
                Month = "October",
                TotalHours = 8,
                HourlyRate = 200
            };

            // Act
            var result = controller.SubmitClaim(claim) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);

            var savedClaim = context.Claims.FirstOrDefault(c => c.LecturerName == "Bob");
            Assert.NotNull(savedClaim);
            Assert.Equal(ClaimStatus.Pending, savedClaim.Status);
        }

        [Fact]
        public void UploadDocuments_NullClaimId_RedirectsToDashboard()
        {
            // Arrange
            var context = GetNewInMemoryDb();
            var controller = GetController(context);

            // Act
            var result = controller.UploadDocuments(null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
        }

        [Fact]
        public void UploadDocuments_ValidFile_AddsDocument()
        {
            // Arrange
            var context = GetNewInMemoryDb();
            var claim = new Claim
            {
                ClaimId = 101,
                LecturerName = "Alice",
                Month = "October",
                TotalHours = 10,
                HourlyRate = 150,
                Status = ClaimStatus.Pending
            };
            context.Claims.Add(claim);
            context.SaveChanges();

            var controller = GetController(context);

            var content = "dummy content";
            var fileName = "file.pdf";
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "supportingFile", fileName);

            // Act
            var result = controller.UploadDocuments(101, file) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);

            var savedDoc = context.Documents.FirstOrDefault(d => d.ClaimId == 101);
            Assert.NotNull(savedDoc);
            Assert.Equal(fileName, savedDoc.FileName);
        }
    }
}