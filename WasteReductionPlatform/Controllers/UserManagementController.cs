using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WasteReductionPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly UserManager<User> _userManager;

	/// <summary>
	/// Initializes a new instance of the UserManagementController class.
	/// </summary>
	/// <param name="userManager">The UserManager instance for managing user data.</param>
	public UserManagementController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users
           .Where(user => !user.IsAdmin)
           .ToList();
        return View(users);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        PopulateProvinces(user.Province);
        return View(user);
    }

	/// <summary>
	/// Updates the user details.
	/// </summary>
	/// <param name="id">The ID of the user to edit.</param>
	/// <param name="model">The User model with updated data.</param>
	/// <returns>The Index view if update succeeds, or the Edit view with validation errors.</returns>
	[HttpPost]
    public async Task<IActionResult> Edit(string id, User model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = model.Email;
        user.UserName = model.Email;
        user.StreetAddress = model.StreetAddress;
        user.City = model.City;
        user.Province = model.Province;
        user.PostalCode = model.PostalCode;
        user.UserType = model.UserType;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        PopulateProvinces(model.Province); // Repopulate provinces in case of validation error

        return View(model);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

	/// <summary>
	/// Deletes a user from the system.
	/// </summary>
	/// <param name="id">The ID of the user to delete.</param>
	/// <param name="model">The User model.</param>
	/// <returns>The Index view if delete succeeds, or the Delete view with error message.</returns>
	[HttpPost]
    public async Task<IActionResult> Delete(string id, User model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "User deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete user.";
        }

        return RedirectToAction(nameof(Index));
    }


    private void PopulateProvinces(string selectedProvince)
    {
    var provinces = new List<SelectListItem>
{
    new SelectListItem { Value = "AB", Text = "Alberta" },
    new SelectListItem { Value = "BC", Text = "British Columbia" },
    new SelectListItem { Value = "MB", Text = "Manitoba" },
    new SelectListItem { Value = "NB", Text = "New Brunswick" },
    new SelectListItem { Value = "NL", Text = "Newfoundland and Labrador" },
    new SelectListItem { Value = "NS", Text = "Nova Scotia" },
    new SelectListItem { Value = "ON", Text = "Ontario" },
    new SelectListItem { Value = "PE", Text = "Prince Edward Island" },
    new SelectListItem { Value = "QC", Text = "Quebec" },
    new SelectListItem { Value = "SK", Text = "Saskatchewan" },
    new SelectListItem { Value = "NT", Text = "Northwest Territories" },
    new SelectListItem { Value = "NU", Text = "Nunavut" },
    new SelectListItem { Value = "YT", Text = "Yukon" }
};


        ViewBag.Provinces = new SelectList(provinces, "Value", "Text", selectedProvince);
    }
}
