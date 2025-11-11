using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Net;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var baseDir = AppContext.BaseDirectory;

// Load config.yml
var configPath = Path.Combine(baseDir, "config.yml");
AppConfig cfg;
if (File.Exists(configPath))
{
    var yaml = await File.ReadAllTextAsync(configPath);
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    cfg = deserializer.Deserialize<AppConfig>(yaml);
}
else
{
    cfg = new AppConfig
    {
        StartSecret = Environment.GetEnvironmentVariable("START_SECRET") ?? "changeme",
        ExeName = "ULTRAKILL.exe",
        WorkingDirectory = baseDir
    };
}

// Serve index.html
var indexHtml = @"<!doctype html>
<html lang=""en"">
<head>
<meta charset=""utf-8"">
<title>Start ULTRAKILL</title>
<style>
body { font-family: system-ui, Arial; max-width:720px; margin:40px auto; }
button { padding:10px 18px; font-size:16px; }
input{ padding:8px; font-size:14px; width:300px; }
#log { margin-top:16px; white-space:pre-wrap; background:#f6f6f6; padding:12px; border-radius:6px; }
</style>
</head>
<body>
<h1>Start ULTRAKILL</h1>
<div>
<label>Secret token: <input id=""secret"" type=""password""/></label>
<button id=""startBtn"">Start ULTRAKILL</button>
</div>
<div id=""log"">Status: idle</div>
<script>
const btn = document.getElementById('startBtn');
const log = document.getElementById('log');
btn.addEventListener('click', async () => {
    const token = document.getElementById('secret').value;
    if (!token) { log.textContent='Enter secret'; return; }
    log.textContent='Sending request...';
    try {
        const res = await fetch('/run',{method:'POST',headers:{'X-Start-Secret': token}});
        const text = await res.text();
        log.textContent = res.ok ? 'Success: '+text : 'Error: '+text;
    } catch(err){ log.textContent='Request failed: '+err; }
});
</script>
</body>
</html>";

app.MapGet("/", async ctx =>
{
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.WriteAsync(indexHtml);
});

app.MapPost("/run", async ctx =>
{
    if (!ctx.Request.Headers.TryGetValue("X-Start-Secret", out var token) ||
        token != cfg.StartSecret)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        await ctx.Response.WriteAsync("Invalid secret");
        return;
    }

    var exePath = Path.Combine(baseDir, cfg.ExeName);
    if (!File.Exists(exePath))
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsync($"Executable not found: {exePath}");
        return;
    }

    try
    {
        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true,
            WorkingDirectory = baseDir,
            CreateNoWindow = false
        };
        Process.Start(psi);
        await ctx.Response.WriteAsync($"Started {cfg.ExeName}");
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsync($"Failed: {ex.Message}");
    }
});

app.Run();

record AppConfig
{
    public string StartSecret { get; set; } = "changeme";
    public string ExeName { get; set; } = "ULTRAKILL.exe";
    public string WorkingDirectory { get; set; } = ".";
}
