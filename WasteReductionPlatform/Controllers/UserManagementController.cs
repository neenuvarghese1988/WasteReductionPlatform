using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WasteReductionPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly UserManager<User> _userManager;

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

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
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
