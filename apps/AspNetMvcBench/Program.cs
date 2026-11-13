var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddControllersWithViews();

var wapp = builder.Build();

wapp.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

wapp.MapGet("/text", () => $"Current time is {DateTime.Now}");

wapp.MapGet("/json", () => new Message(msg: "Hello"));

wapp.Run();

public sealed record Message(string msg);
