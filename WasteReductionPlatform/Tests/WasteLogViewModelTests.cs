using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WasteReductionPlatform.ViewModels;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.Tests
{
    [TestFixture]
    public class WasteLogViewModelTests
    {
        [Test]
        public void WasteLogViewModel_ValidModel_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var model = new WasteLogViewModel
            {
                Id = 1,
                Date = DateTime.Now,
                Weight = 10.5,
                WasteType = "Plastic",
                WasteTypes = new List<string> { "Plastic", "Paper", "Glass" },
                UserType = UserType.Residential
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Empty, "Model should be valid and have no validation errors.");
        }

        [Test]
        public void WasteLogViewModel_InvalidModel_ShouldHaveValidationErrors()
        {
            // Arrange
            var model = new WasteLogViewModel
            {
                Id = 1,
                Date = null,
                Weight = -1,
                WasteType = null,
                WasteTypes = new List<string>(),
                UserType = null
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Not.Empty, "Model should be invalid and have validation errors.");
            Assert.That(validationResults, Has.Exactly(3).Items); // Date, Weight, and WasteType should have errors
        }

        [Test]
        public void WasteLogViewModel_InvalidWeight_ShouldHaveValidationError()
        {
            // Arrange
            var model = new WasteLogViewModel
            {
                Id = 1,
                Date = DateTime.Now,
                Weight = 0,
                WasteType = "Plastic",
                WasteTypes = new List<string> { "Plastic" },
                UserType = UserType.Residential
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Not.Empty, "Model should have a validation error for Weight.");
            Assert.That(validationResults, Has.Exactly(1).Items);
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo("Please enter a valid weight in kilograms."));
        }

        private IList<ValidationResult> ValidateModel(WasteLogViewModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}