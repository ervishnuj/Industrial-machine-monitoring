using IndustrialMachineMonitoring.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertLogsController : ControllerBase
{
    private readonly MachineDbContext _context;

    public AlertLogsController(MachineDbContext context)
    {
        _context = context;
    }

    // GET: api/AlertLogs
    [HttpGet]
    public async Task<IActionResult> GetAllAlerts()
    {
        var alerts = await _context.AlertLogs
            .Include(a => a.Machine)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(alerts);
    }

    // GET: api/AlertLogs/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(int id)
    {
        var alert = await _context.AlertLogs
            .Include(a => a.Machine)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alert == null)
            return NotFound();

        return Ok(alert);
    }

    // GET: api/AlertLogs/machine/1
    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetAlertsByMachine(int machineId)
    {
        var alerts = await _context.AlertLogs
            .Where(a => a.MachineId == machineId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(alerts);
    }

    // PUT: api/AlertLogs/1/acknowledge
    [HttpPut("{id}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlert(int id)
    {
        var alert = await _context.AlertLogs.FindAsync(id);

        if (alert == null)
            return NotFound();

        if (alert.IsAcknowledged)
        {
            return Ok(new
            {
                Message = "Alert already acknowledged",
                Alert = alert
            });
        }

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Alert acknowledged successfully",
            Alert = alert
        });
    }
}