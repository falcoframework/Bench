var bldr = WebApplication.CreateSlimBuilder(args);

bldr.Logging.ClearProviders();
var wapp = bldr.Build();

wapp.MapGet("/", () => Results.RazorSlice<RazorSlicesBench.Views.Hello, DateTime>(DateTime.UtcNow));
wapp.MapGet("/text", () => Message.Create().msg);
wapp.MapGet("/json", () => Message.Create());

wapp.Run();

public sealed record Message(string msg)
{
    public static Message Create() => new($"Current time is {DateTime.UtcNow:R}");
}