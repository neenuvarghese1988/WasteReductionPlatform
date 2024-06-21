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
                // Server-side validation for pickup date
                if (model.PickupDate < DateTime.Today)
                {
                    ModelState.AddModelError("PickupDate", "Pickup date must not be in the past.");
                }
                // Check if a schedule with the same PickupDate, PickupType, UserType, and Area already exists
                var existingSchedule = await _context.PickupSchedules
                    .FirstOrDefaultAsync(s => s.PickupDate == model.PickupDate &&
                                              s.PickupType == model.PickupType &&
                                              s.UserType == model.UserType &&
                                              s.Area == model.Area);
                if (existingSchedule != null)
                {
                    TempData["Error"] = "A schedule with the same details already exists.";
                    return View(model);
                }

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

                   TempData["Success"] = "Pickup schedule created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
          //  TempData["Error"] = "Failed to create pickup schedule. Please check the form for errors.";
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
                // Server-side validation for pickup date
                if (model.PickupDate < DateTime.Today)
                {
                    ModelState.AddModelError("PickupDate", "Pickup date must not be in the past.");
                }

                //// Check if a schedule with the same PickupDate, PickupType, UserType, and Area already exists
                //var existingSchedule = await _context.PickupSchedules
                //    .FirstOrDefaultAsync(s => s.PickupDate == model.PickupDate &&
                //                              s.PickupType == model.PickupType &&
                //                              s.UserType == model.UserType &&
                //                              s.Area == model.Area);
                //if (existingSchedule != null)
                //{
                //    TempData["Error"] = "A schedule with the same details already exists.";
                //    return View(model);
                //}
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

                       TempData["Success"] = "Pickup schedule updated successfully.";
                        return RedirectToAction(nameof(Index));
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
                }
            }
           // TempData["Error"] = "Failed to update pickup schedule. Please check the form for errors.";
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

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule != null)
            {
                _context.PickupSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pickup schedule deleted successfully.";
            }
            else
            {
               TempData["Error"] = "Failed to delete pickup schedule. It may have already been removed.";
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
