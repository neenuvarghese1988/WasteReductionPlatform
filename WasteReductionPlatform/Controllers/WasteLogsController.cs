using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteReductionPlatform.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.ViewModels;

[Authorize(Roles = "User")]
public class WasteLogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public WasteLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var logs = await _context.WasteLogs
            .Where(w => w.UserId == userId)
            .ToListAsync();
        return View(logs);
    }

    public IActionResult Create()
    {
        return View();
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
            UserType = log.UserType
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
                log.Date = model.Date;
                log.Weight = model.Weight;
                log.WasteType = model.WasteType;
                log.UserType = model.UserType;
                _context.Update(log);
                await _context.SaveChangesAsync();
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

        var log = await _context.WasteLogs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (log == null)
        {
            return NotFound();
        }

        return View(log);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var log = await _context.WasteLogs.FindAsync(id);
        _context.WasteLogs.Remove(log);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool WasteLogExists(int id)
    {
        return _context.WasteLogs.Any(e => e.Id == id);
    }
}
