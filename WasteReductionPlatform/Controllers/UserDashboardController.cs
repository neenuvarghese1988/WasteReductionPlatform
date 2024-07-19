using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.Controllers
{
    [Authorize(Roles = "User")]
    public class UserDashboardController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;

		public UserDashboardController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			ViewBag.UserType = HttpContext.Session.GetString("UserType");

			var unreadNotificationsCount = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
			HttpContext.Session.SetInt32("UnreadNotificationsCount", unreadNotificationsCount);
			return View();
		}
	}
}
