using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ToolController : ControllerBase
{
    [HttpPost("run")]
    public IActionResult RunTool([FromBody] string args)
    {
        // Determine the path of the .exe
        var exeName = "ULTRAKILL.exe";
        // Use the base directory of the application
        var exePath = Path.Combine(AppContext.BaseDirectory, exeName);

        if (!System.IO.File.Exists(exePath))
            return NotFound($"Executable not found at {exePath}");

        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(exePath)
        };

        using (var process = Process.Start(psi))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return Ok(new {
                ExitCode = process.ExitCode,
                StdOut = output,
                StdErr = error
            });
        }
    }
}
