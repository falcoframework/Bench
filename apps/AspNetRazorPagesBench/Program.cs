var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapRazorPages();

app.Run();
