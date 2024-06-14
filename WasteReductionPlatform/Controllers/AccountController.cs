using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Humanizer;
using System.Drawing.Drawing2D;

namespace WasteReductionPlatform.Controllers
{
    public class AccountController : Controller
    {
		//Manages user-related operations like creating, updating, and finding users. It's part of ASP.NET Core Identity.
		private readonly UserManager<User> _userManager;
		//Manages sign-in operations, such as checking passwords and signing users in or out.
		private readonly SignInManager<User> _signInManager;
		//it's used for sending email confirmation links to users.
		private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
		{
			return View();
		}

		//asynchronous operation- Allows to perform operations without blocking the main thread, making the application more responsive,
		//especially during I/O-bound operations like sending emails or database access.

		/// <summary>
		/// Handles the registration of a new user.
		/// - Creates a new user account.
		/// - Adds the user to the "User" role.
		/// - Sends an email confirmation link.
		/// - Redirects to the login page with a success message or shows errors if the registration fails.
		/// POST: /Account/Register
		/// </summary>
		[HttpPost]

		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    UserType = model.UserType,
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    Province = model.Province,
                    PostalCode = model.PostalCode
                };
                var result = await _userManager.CreateAsync(user, model.Password);//await these tasks to continue execution only after these operations complete.

				if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    // Send email confirmation link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code }, protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

                    TempData["Success"] = "Registration successful. Please check your email to confirm your account.";
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

		/// <summary>
		/// Confirms the user's email address.
		/// - Validates the confirmation token.
		/// - If successful, marks the email as confirmed and redirects to the login page with a success message.
		/// - If unsuccessful, shows an error message.
		/// GET: /Account/ConfirmEmail
		/// </summary>
		[HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["Success"] = "Email confirmed successfully. You can now log in.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Error"] = "Error confirming your email.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

		/// <summary>
		/// Handles user login.
		/// - Checks if the user exists and if the email is confirmed.
		/// - Attempts to sign the user in.
		/// - Redirects to the appropriate dashboard based on the user's role.
		/// - Shows error messages if login fails.
		/// POST: /Account/Login
		/// </summary>
		[HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {

                    // Check if email is confirmed
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                        return View(model);
                    }

                    // Try to sign in
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("Index", "AdminDashboard");
                        }
                        else
                        {
                            return RedirectToAction("Index", "UserDashboard");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email address does not exist.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
