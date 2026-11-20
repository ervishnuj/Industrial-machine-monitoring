using Microsoft.AspNetCore.Mvc;

namespace IndustrialMachineMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Application = "Industrial Machine Monitoring System",
            Status = "Running",
            Technology = "ASP.NET Core Web API",
            Database = "MySQL"
        });
    }
}