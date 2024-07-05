using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;

[Authorize(Roles = "User")]
    public class UserPickupRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserPickupRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserPickupRequests
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPickupRequests = await _context.UserPickupRequests
                .Include(u => u.PickupSchedule)
                .Where(u => u.UserId == userId)
                .ToListAsync();
            return View(userPickupRequests);
        }

        // GET: UserPickupRequests/Create
        public async Task<IActionResult> Schedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            var schedules = await _context.PickupSchedules
                .Where(p => p.Area == user.City && p.UserType == user.UserType)
                .ToListAsync();

            return View(schedules);
        }

        // POST: UserPickupRequests/Create
        [HttpPost]
        public async Task<IActionResult> Create(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pickupSchedule = await _context.PickupSchedules.FindAsync(id);

            if (pickupSchedule == null)
            {
                return NotFound();
            }

            var userPickupRequest = new UserPickupRequest
            {
                PickupScheduleId = pickupSchedule.Id,
                UserId = userId,
                RequestedDate = DateTime.Now,
                IsConfirmed = false
            };

            _context.UserPickupRequests.Add(userPickupRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pickup request created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserPickupRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPickupRequest = await _context.UserPickupRequests
                .Include(u => u.PickupSchedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPickupRequest == null)
            {
                return NotFound();
            }

            return View(userPickupRequest);
        }

        // POST: UserPickupRequests/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userPickupRequest = await _context.UserPickupRequests.FindAsync(id);
            _context.UserPickupRequests.Remove(userPickupRequest);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Pickup request deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool UserPickupRequestExists(int id)
        {
            return _context.UserPickupRequests.Any(e => e.Id == id);
        }
    }

