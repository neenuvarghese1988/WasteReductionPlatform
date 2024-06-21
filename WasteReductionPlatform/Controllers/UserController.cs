using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Humanizer;
using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WasteReductionPlatform.Controllers
{
    public class UserController : Controller
    {
        //Manages user-related operations like creating, updating, and finding users. It's part of ASP.NET Core Identity.
        private readonly UserManager<User> _userManager;
        //Manages sign-in operations, such as checking passwords and signing users in or out.
        private readonly SignInManager<User> _signInManager;
        //it's used for sending email confirmation links to users.
        private readonly IEmailSender _emailSender;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
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

		
		[HttpPost]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
			//if (model.Role == "Admin")
			//{
			//	model.UserType = UserType.Residential;
			//	// Clear the model state to update it
			//	ModelState.Clear();

			//	// Revalidate specific fields if necessary
			//	TryValidateModel(model);
			//}

			if (ModelState.IsValid)
            {
                //// Additional check to restrict multiple admin creation
                //var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                //if (model.Role == "Admin" && adminUsers.Count > 0)
                //{
                //    ModelState.AddModelError("", "An admin account already exists. Additional admin accounts require approval.");
                //    return View(model);
                //}

                var user = new User
                {
                    UserName = model.Username,
                    //IsAdmin = model.Role == "Admin",
                    UserType = model.UserType,
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    Province = model.Province,
                    PostalCode = model.PostalCode
                };
                var result = await _userManager.CreateAsync(user, model.Password);//await these tasks to continue execution only after these operations complete.

                if (result.Succeeded)
                {
                   // await _userManager.AddToRoleAsync(user, model.Role);

                    TempData["Success"] = "Registration successful.";
                    return RedirectToAction("Login", "User");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
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

        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists by username
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    // Try to sign in
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
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
                    ModelState.AddModelError(string.Empty, "Username does not exist.");
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
