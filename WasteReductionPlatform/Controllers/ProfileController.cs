using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
}
