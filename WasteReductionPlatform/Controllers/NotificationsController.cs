using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;

[Authorize(Roles = "User")]
public class NotificationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var alerts = await _context.Notifications
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return View(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var alert = await _context.Notifications.FindAsync(id);
        if (alert == null || alert.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return NotFound();
        }

        alert.IsRead = true;
        _context.Update(alert);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<bool> HasUnreadNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .AnyAsync(n => n.UserId == userId && !n.IsRead);
    }

}
