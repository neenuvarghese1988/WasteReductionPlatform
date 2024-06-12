using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WasteReductionPlatform.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ApplicationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> MissedPickup()
        {
            try
            {
                // Fetch pickups that are not confirmed and have a past date
                var missedPickups = await _context.PickupSchedules
                    .Where(p => !p.IsConfirmed && p.PickupDate < DateTime.Today)
                    .ToListAsync();

                // Optional: Send notifications to users (not shown in this example)

                return View(missedPickups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching missed pickups.");
                TempData["Error"] = "An error occurred while fetching missed pickups.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> HighWasteAlerts()
        {
            try
            {
                // Fetch logs where the weight exceeds a threshold
                var highWasteLogs = await _context.WasteLogs
                    .Where(w => w.Weight > 100) // Example threshold
                    .ToListAsync();

                // Optional: Send alerts to users (not shown in this example)

                return View(highWasteLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching high waste alerts.");
                TempData["Error"] = "An error occurred while fetching high waste alerts.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
