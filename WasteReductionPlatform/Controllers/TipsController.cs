using Microsoft.AspNetCore.Mvc;

public class TipsController : Controller
{
    public IActionResult Index()
    {
        // Display waste reduction tips
        return View();
    }
}
