using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;

namespace WasteReductionPlatform.Tests
{
    [TestFixture]
    public class UserProfileViewModelTests
    {
        [Test]
        public void UserProfileViewModel_ValidModel_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var model = new UserProfileViewModel
            {
                Email = "test@example.com",
                UserType = UserType.Commercial,
                StreetAddress = "60 Centreville Street",
                City = "Kitchener",
                Province = "ON",
                PostalCode = "N2A1R9"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Empty, "Model should be valid and have no validation errors.");
        }

        [Test]
        public void UserProfileViewModel_InvalidModel_ShouldHaveValidationErrors()
        {
            // Arrange
            var model = new UserProfileViewModel
            {
                // Intentionally leaving required fields empty
                Email = null,
                UserType = null,
                StreetAddress = null,
                City = null,
                Province = null,
                PostalCode = null
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.That(validationResults, Is.Not.Empty, "Model should be invalid and have validation errors.");
        }

        private IList<ValidationResult> ValidateModel(UserProfileViewModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}