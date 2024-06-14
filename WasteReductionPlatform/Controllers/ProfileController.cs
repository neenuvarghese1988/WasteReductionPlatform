using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;

    public ProfileController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserProfileViewModel
        {
            Email = user.Email,
            UserType = user.UserType,
            StreetAddress = user.StreetAddress,
            City = user.City,
            Province = user.Province,
            PostalCode = user.PostalCode
        };
        PopulateProvinces(user.Province);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(UserProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.UserType = model.UserType;
            user.StreetAddress = model.StreetAddress;
            user.City = model.City;
            user.Province = model.Province;
            user.PostalCode = model.PostalCode;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        PopulateProvinces(model.Province); // Repopulate provinces in case of validation error
        return View(model);
    }

    private void PopulateProvinces(string selectedProvince)
    {
        var provinces = new List<SelectListItem>
    {
        new SelectListItem { Value = "Alberta", Text = "Alberta" },
        new SelectListItem { Value = "British Columbia", Text = "British Columbia" },
        new SelectListItem { Value = "Manitoba", Text = "Manitoba" },
        new SelectListItem { Value = "New Brunswick", Text = "New Brunswick" },
        new SelectListItem { Value = "Newfoundland and Labrador", Text = "Newfoundland and Labrador" },
        new SelectListItem { Value = "Nova Scotia", Text = "Nova Scotia" },
        new SelectListItem { Value = "Ontario", Text = "Ontario" },
        new SelectListItem { Value = "Prince Edward Island", Text = "Prince Edward Island" },
        new SelectListItem { Value = "Quebec", Text = "Quebec" },
        new SelectListItem { Value = "Saskatchewan", Text = "Saskatchewan" },
        new SelectListItem { Value = "Northwest Territories", Text = "Northwest Territories" },
        new SelectListItem { Value = "Nunavut", Text = "Nunavut" },
        new SelectListItem { Value = "Yukon", Text = "Yukon" }
    };

        ViewBag.Provinces = new SelectList(provinces, "Value", "Text", selectedProvince);
    }
}
