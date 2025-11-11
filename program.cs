using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[ApiController]
[Route("[controller]")]
public class ToolController : ControllerBase
{
    [HttpGet("run")]
    public IActionResult RunTool()
    {
        try
        {
            var exePath = Path.Combine(AppContext.BaseDirectory, "ULTRAKILL.exe");

            var processInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "", // Add any arguments here
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                    return BadRequest(error);

                return Ok(output); // return EXE output to the user
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
