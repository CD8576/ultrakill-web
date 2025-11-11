using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToolController : ControllerBase
    {
        [HttpPost("run")]
        public IActionResult RunExe([FromQuery] string args = "")
        {
            // Make sure your EXE is in the base directory
            string exePath = Path.Combine(AppContext.BaseDirectory, "ULTRAKILL.exe");
            
            if (!System.IO.File.Exists(exePath))
                return NotFound("EXE file not found.");

            try
            {
                var process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = args;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                return Ok(new { Output = output, Error = error, ExitCode = process.ExitCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
