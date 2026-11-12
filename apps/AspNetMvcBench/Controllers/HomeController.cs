using Microsoft.AspNetCore.Mvc;

namespace AspNetMvcBench.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => View(DateTime.UtcNow);
}
