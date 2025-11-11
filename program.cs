var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Serve static files from wwwroot
app.UseDefaultFiles(); // looks for index.html by default
app.UseStaticFiles();

// Default route at /
app.MapGet("/", () => "Hello! The API is running.");

// Map your existing controllers
app.MapControllers();

// Make sure the app listens on all interfaces (needed for Koyeb)
app.Urls.Add("http://+:80");

app.Run();
