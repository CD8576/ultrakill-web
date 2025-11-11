var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Default route at /
app.MapGet("/", () => "Hello! The API is running.");

// Map your existing controllers
app.MapControllers();

// Make sure the app listens on all interfaces (needed for Koyeb)
app.Urls.Add("http://+:8080");

app.Run();
