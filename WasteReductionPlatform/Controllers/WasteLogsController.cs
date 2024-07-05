using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WasteReductionPlatform.Controllers
{
    [Authorize(Roles = "User")]
    public class WasteLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public WasteLogsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var logs = await _context.WasteLogs
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(logs);
        }

        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get the user ID from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve the user using UserManager
                var user = await _userManager.FindByIdAsync(userId);

                // Check if user is null (defensive check)
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                // Query the database to get the UserType based on the email
                var userType = user.UserType;

                // Create the model and populate the WasteTypes
                var model = new WasteLogViewModel
                {
                    UserType = userType,
                    WasteTypes = GetWasteTypes(userType.ToString())
                };

                return View(model);
            }
            else
            {
                // If the user is not authenticated, redirect to the home page or login
                return RedirectToAction("Index", "Home");
            }
        }


        // Example method to determine the waste types based on the user type
        private List<string> GetWasteTypes(string userType)
        {
            if (userType == "Residential")
            {
                return new List<string>
                {
                    "Blue Box (Recyclables)",
                    "Green Cart (Organic Waste)",
                    "Garbage",
                    "Bulk Waste",
                    "Hazardous Waste"
                };
            }
            else if (userType == "Commercial")
            {
                return new List<string>
                {
                    "General Waste (Garbage)",
                    "Paper and Cardboard",
                    "Plastics",
                    "Glass",
                    "Metals",
                    "Organic Waste",
                    "Construction and Demolition (C&D)",
                    "Electronic Waste",
                    "Medical and Clinical Waste",
                    "Confidential Paper",
                    "Textiles",
                    "Grease and Oils",
                    "Wood Waste",
                    "Chemical Waste",
                    "Industrial Waste"
                };
            }

            return new List<string>();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WasteLogViewModel model)
        {
            
            if (ModelState.IsValid)
            {
               
                    var log = new WasteLog
                    {
                        Date = model.Date,
                        Weight = model.Weight,
                        WasteType = model.WasteType,
                        UserType = model.UserType,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Waste log created successfully.";
                    return RedirectToAction(nameof(Index));
                }

            // TempData["Error"] = "Failed to create waste log. Please check the form for errors.";
            // Repopulate WasteTypes if the form submission fails
            model.WasteTypes = GetWasteTypes(model.UserType.ToString());
            return View(model);
        }

 
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.WasteLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            var model = new WasteLogViewModel
            {
                Id = log.Id,
                Date = log.Date,
                Weight = log.Weight,
                WasteType = log.WasteType,
                UserType = log.UserType,
                WasteTypes = GetWasteTypes(log.UserType.ToString())
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, WasteLogViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var log = await _context.WasteLogs.FindAsync(id);
                    log.Date = model.Date.Value; // Date is nullable in ViewModel, so use .Value
                    log.Weight = model.Weight.Value; // Weight is nullable in ViewModel, so use .Value
                    log.WasteType = model.WasteType;
                    log.UserType = model.UserType;

                    _context.Update(log);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Waste log updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WasteLogExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Repopulate WasteTypes if the form submission fails
            model.WasteTypes = GetWasteTypes(model.UserType.ToString());
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.WasteLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var log = await _context.WasteLogs.FindAsync(id);
            if (log != null)
            {
                _context.WasteLogs.Remove(log);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Waste log deleted successfully.";
            }
            else
            {
                TempData["ErrorWasteLog"] = "Failed to delete waste log. It may have already been removed.";
            }
            return RedirectToAction(nameof(Index));
        }


        private bool WasteLogExists(int id)
        {
            return _context.WasteLogs.Any(e => e.Id == id);
        }
    }
}
