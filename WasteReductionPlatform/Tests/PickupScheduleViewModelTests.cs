using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WasteReductionPlatform.ViewModels;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.Tests
{
    [TestFixture]
    public class PickupScheduleViewModelTests
    {
        [Test]
        public void PickupScheduleViewModel_ValidData_ShouldPassValidation()
        {
            // Arrange
            var model = new PickupScheduleViewModel
            {
                Id = 1,
                PickupDate = DateTime.Now.AddDays(1),
                PickupType = "Recyclables",
                UserType = UserType.Residential,
                Area = "Downtown",
                IsConfirmed = true
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Empty);
        }
        [Test]
        public void PickupScheduleViewModel_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var model = new PickupScheduleViewModel
            {
                // Intentionally leaving required fields empty or invalid
                PickupDate = default, // This will trigger validation for required date
                PickupType = null,    // This will trigger validation for required pickup type
                UserType = default,   // This will trigger validation for required user type
                Area = null           // This will trigger validation for required area
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Has.Count.EqualTo(4), "Expected 4 validation errors");
            Assert.That(validationResults, Has.Some.Matches<ValidationResult>(v => v.MemberNames.Contains("PickupDate")), "Expected validation error for PickupDate");
            Assert.That(validationResults, Has.Some.Matches<ValidationResult>(v => v.MemberNames.Contains("PickupType")), "Expected validation error for PickupType");
            Assert.That(validationResults, Has.Some.Matches<ValidationResult>(v => v.MemberNames.Contains("UserType")), "Expected validation error for UserType");
            Assert.That(validationResults, Has.Some.Matches<ValidationResult>(v => v.MemberNames.Contains("Area")), "Expected validation error for Area");
        }

        [Test]
        public void PickupScheduleViewModel_PastPickupDate_ShouldFailValidation()
        {
            // Arrange
            var model = new PickupScheduleViewModel
            {
                PickupDate = DateTime.Now.AddDays(-1),
                PickupType = "Recyclables",
                UserType = UserType.Residential,
                Area = "Downtown"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults, Has.Some.Matches<ValidationResult>(v => v.MemberNames.Contains("PickupDate")));
        }

        [Test]
        public void PickupScheduleViewModel_SetAndGetProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var model = new PickupScheduleViewModel();
            var expectedDate = DateTime.Now.Date.AddDays(1);
            var expectedType = "Organic";
            var expectedUserType = UserType.Commercial;
            var expectedArea = "Suburb";
            var expectedIsConfirmed = true;

            // Act
            model.Id = 5;
            model.PickupDate = expectedDate;
            model.PickupType = expectedType;
            model.UserType = expectedUserType;
            model.Area = expectedArea;
            model.IsConfirmed = expectedIsConfirmed;

            // Assert
            Assert.That(model.Id, Is.EqualTo(5));
            Assert.That(model.PickupDate, Is.EqualTo(expectedDate));
            Assert.That(model.PickupType, Is.EqualTo(expectedType));
            Assert.That(model.UserType, Is.EqualTo(expectedUserType));
            Assert.That(model.Area, Is.EqualTo(expectedArea));
            Assert.That(model.IsConfirmed, Is.EqualTo(expectedIsConfirmed));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}