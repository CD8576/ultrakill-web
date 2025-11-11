using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToolController : ControllerBase
    {
        [HttpGet("run")]
        public IActionResult RunTool()
        {
            var exePath = Path.Combine(AppContext.BaseDirectory, "ULTRAKILL.exe");

            if (!System.IO.File.Exists(exePath))
                return NotFound("Executable not found.");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = "", // add arguments if needed
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            return Ok(output);
        }
    }
}
