var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddRazorPages();

var wapp = builder.Build();

wapp.MapRazorPages();

wapp.MapGet("/text", () => $"Current time is {DateTime.Now}");

wapp.MapGet("/json", () => new Message(msg: "Hello"));

wapp.Run();

public sealed record Message(string msg);