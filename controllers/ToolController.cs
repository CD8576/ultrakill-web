using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[ApiController]
[Route("[controller]")]
public class ToolController : ControllerBase
{
    [HttpGet("run")]
    public IActionResult RunExe()
    {
        var process = new Process();
        process.StartInfo.FileName = "/app/YourApp.exe"; // path inside Docker
        process.StartInfo.Arguments = ""; // any arguments
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return Content(output); // send output back to browser
    }
}
