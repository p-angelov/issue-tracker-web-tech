using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IssuesController : ControllerBase
{
    [HttpGet("/health")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok("Service is running");
    }
}