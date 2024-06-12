using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;

namespace WasteReductionPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PickupSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PickupSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PickupSchedules.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PickupScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schedule = new PickupSchedule
                {
                    PickupDate = model.PickupDate,
                    PickupType = model.PickupType,
                    UserType = model.UserType,
                    Area = model.Area
                };
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var model = new PickupScheduleViewModel
            {
                Id = schedule.Id,
                PickupDate = schedule.PickupDate,
                PickupType = schedule.PickupType,
                UserType = schedule.UserType,
                Area = schedule.Area
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PickupScheduleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var schedule = await _context.PickupSchedules.FindAsync(id);
                    schedule.PickupDate = model.PickupDate;
                    schedule.PickupType = model.PickupType;
                    schedule.UserType = model.UserType;
                    schedule.Area = model.Area;
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PickupScheduleExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.PickupSchedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: PickupSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule != null)
            {
                _context.PickupSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PickupScheduleExists(int id)
        {
            return _context.PickupSchedules.Any(e => e.Id == id);
        }
        [Authorize]
        public async Task<IActionResult> Schedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            var schedules = await _context.PickupSchedules
                .Where(p => p.Area == user.PostalCode && p.UserType == user.UserType)
                .ToListAsync();

            return View(schedules);
        }

        [Authorize]
        public async Task<IActionResult> RequestPickup(int id)
        {
            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            // Implement logic to handle the pickup request, e.g., save a record of the request
            return RedirectToAction(nameof(Schedule));
        }

    }
}