using NUnit.Framework;
using Moq;
using WasteReductionPlatform.Controllers;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;


namespace WasteReductionPlatform.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<IEmailSender> _mockEmailSender;
        private UserController _controller;

        [OneTimeSetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null, null, null, null);

            _mockEmailSender = new Mock<IEmailSender>();

            _controller = new UserController(_mockUserManager.Object, _mockSignInManager.Object, _mockEmailSender.Object);
        }

        [Test]
        public async Task Register_ValidUser_ReturnsRedirectToAction()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "Blessymol",
                Password = "Achu@222016",
                ConfirmPassword = "Achu@222016",
                UserType = UserType.Commercial,
                StreetAddress = "60 Centreville Street",
                City = "Kitchener",
                Province = "ON",
                PostalCode = "N2A1R9"
            };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Clear any existing ModelState errors
            _controller.ModelState.Clear();

            // Act
            IActionResult result = null;
            try
            {
                result = await _controller.Register(model);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown during registration: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>(), "Result should be RedirectToActionResult");

            if (result is RedirectToActionResult redirectResult)
            {
                Assert.That(redirectResult.ActionName, Is.EqualTo("Login"), "Should redirect to Login action");
                Assert.That(redirectResult.ControllerName, Is.EqualTo("User"), "Should redirect to User controller");
            }
            else
            {
                var resultType = result.GetType().Name;
                var viewResult = result as ViewResult;
                var modelState = _controller.ModelState;
                var modelStateErrors = string.Join(", ", modelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                Assert.Fail($"Expected RedirectToActionResult, but got {resultType}. " +
                            $"ModelState.IsValid: {modelState.IsValid}. " +
                            $"ModelState Errors: {modelStateErrors}. " +
                            $"ViewData: {(viewResult?.ViewData != null ? string.Join(", ", viewResult.ViewData.Select(kvp => $"{kvp.Key}: {kvp.Value}")) : "null")}");
            }

            // Verify that CreateAsync was called with the correct parameters
            _mockUserManager.Verify(um => um.CreateAsync(It.Is<User>(u =>
                u.UserName == model.Username &&
                u.Email == model.Username &&
                u.UserType == model.UserType.Value &&
                u.StreetAddress == model.StreetAddress &&
                u.City == model.City &&
                u.Province == model.Province &&
                u.PostalCode == model.PostalCode
            ), model.Password), Times.Once);

            // Verify that AddToRoleAsync was called
            _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.Is<string>(r => r == "Commercial")), Times.Once);
        }

        [Test]
        public async Task Register_InvalidUser_ReturnsViewWithModel()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "testuser",
                Password = "Password123",
                UserType = UserType.Residential,
                StreetAddress = "123 Main St",
                City = "Test City",
                Province = "Test Province",
                PostalCode = "A1A 1A1"
            };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User already exists" }));

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task Login_ValidUser_ReturnsRedirectToAction()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "Blessyjithu",
                Password = "Achu@2016",
                RememberMe = true
            };

            var user = new User { UserName = model.Username };
            _mockUserManager.Setup(um => um.FindByNameAsync(model.Username)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("UserDashboard"));
        }

        [Test]
        public async Task Login_InvalidUser_ReturnsViewWithModel()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "Blessyjithu",
                Password = "Achuachu",
                RememberMe = true
            };

            var user = new User { UserName = model.Username };
            _mockUserManager.Setup(um => um.FindByNameAsync(model.Username)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task Login_LockedOutUser_ReturnsLockoutView()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "Blessyjithu",
                Password = "Achu@2016",
                RememberMe = true
            };

            var user = new User { UserName = model.Username };
            _mockUserManager.Setup(um => um.FindByNameAsync(model.Username)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Lockout"));
        }

        [Test]
        public async Task Register_EmailNotConfirmed_ReturnsViewWithError()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "testuser",
                Password = "Password123",
                UserType = UserType.Residential,
                StreetAddress = "123 Main St",
                City = "Test City",
                Province = "Test Province",
                PostalCode = "A1A 1A1"
            };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<User>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo("You need to confirm your email address before logging in."));
        }

        [Test]
        public async Task Register_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "testuser",
                Password = "Password123",
                ConfirmPassword = "Password1234", // Mismatched passwords
                UserType = UserType.Residential,
                StreetAddress = "123 Main St",
                City = "Test City",
                Province = "Test Province",
                PostalCode = "A1A 1A1"
            };

            // Act
            _controller.ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
            var result = await _controller.Register(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.Count, Is.GreaterThan(0));
        }
    }
}
