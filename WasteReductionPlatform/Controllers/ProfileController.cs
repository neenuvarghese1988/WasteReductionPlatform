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
