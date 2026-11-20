
using IndustrialMachineMonitoring.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineAlertsController : ControllerBase
{
    private readonly MachineDbContext _context;

    public MachineAlertsController(MachineDbContext context)
    {
        _context = context;
    }

    [HttpGet("{machineId}")]
    public async Task<IActionResult> GetMachineAlerts(int machineId)
    {
        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.Id == machineId);

        if (machine == null)
        {
            return NotFound(new
            {
                Message = $"Machine ID {machineId} not found."
            });
        }

        var latestReading = await _context.SensorReadings
            .Where(r => r.MachineId == machineId)
            .OrderByDescending(r => r.RecordedAt)
            .FirstOrDefaultAsync();

        if (latestReading == null)
        {
            return NotFound(new
            {
                Message = "No sensor readings available for this machine."
            });
        }

        var alerts = new List<string>();

        // Demo thresholds only. Actual limits depend on machine specifications.
        if (latestReading.Temperature > 80)
            alerts.Add("HIGH TEMPERATURE");

        if (latestReading.MotorCurrent > 15)
            alerts.Add("HIGH MOTOR CURRENT");

        if (latestReading.Vibration > 5)
            alerts.Add("HIGH VIBRATION");

        return Ok(new
        {
            MachineId = machine.Id,
            MachineName = machine.MachineName,
            LatestReading = latestReading,
            Status = alerts.Count == 0 ? "NORMAL" : "WARNING",
            Alerts = alerts
        });
    }
}
