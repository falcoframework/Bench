using Microsoft.AspNetCore.Mvc;

namespace AspNetMvcBench.Controllers;

public sealed record Message(string msg);

public sealed class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View(DateTime.UtcNow);
}
