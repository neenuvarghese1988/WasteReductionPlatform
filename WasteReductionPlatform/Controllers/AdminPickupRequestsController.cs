using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;


    [Authorize(Roles = "Admin")]
    public class AdminPickupRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPickupRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPickupRequests
        public async Task<IActionResult> Index()
        {
            var pickupRequests = await _context.UserPickupRequests
                .Include(u => u.PickupSchedule)
                .Include(u => u.User)
                .ToListAsync();
            return View(pickupRequests);
        }

        // GET: AdminPickupRequests/Confirm/5
        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPickupRequest = await _context.UserPickupRequests
                .Include(u => u.PickupSchedule)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPickupRequest == null)
            {
                return NotFound();
            }

            return View(userPickupRequest);
        }

        // POST: AdminPickupRequests/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var userPickupRequest = await _context.UserPickupRequests.FindAsync(id);
            if (userPickupRequest == null)
            {
                return NotFound();
            }

            userPickupRequest.IsConfirmed = true;
            _context.Update(userPickupRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pickup request confirmed successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool UserPickupRequestExists(int id)
        {
            return _context.UserPickupRequests.Any(e => e.Id == id);
        }
    }

