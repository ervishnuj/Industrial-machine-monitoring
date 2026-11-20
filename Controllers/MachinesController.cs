using IndustrialMachineMonitoring.Data;
using IndustrialMachineMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly MachineDbContext _context;

    public MachinesController(MachineDbContext context)
    {
        _context = context;
    }

    // 1. Register a new machine
    [HttpPost]
    public async Task<IActionResult> CreateMachine(Machine machine)
    {
        machine.LastUpdated = DateTime.UtcNow;

        _context.Machines.Add(machine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMachineById),
            new { id = machine.Id },
            machine
        );
    }

    // 2. Get all machines
    [HttpGet]
    public async Task<IActionResult> GetAllMachines()
    {
        var machines = await _context.Machines.ToListAsync();

        return Ok(machines);
    }

    // 3. Get machine by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMachineById(int id)
    {
        var machine = await _context.Machines.FindAsync(id);

        if (machine == null)
        {
            return NotFound(new
            {
                Message = $"Machine with ID {id} not found"
            });
        }

        return Ok(machine);
    }

    // 4. Update machine
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMachine(int id, Machine updatedMachine)
    {
        var machine = await _context.Machines.FindAsync(id);

        if (machine == null)
        {
            return NotFound(new
            {
                Message = $"Machine with ID {id} not found"
            });
        }

        var codeExists = await _context.Machines.AnyAsync(m =>
       m.MachineCode == updatedMachine.MachineCode &&
       m.Id != id
   );

        if (codeExists)
        {
            return Conflict(new
            {
                Message = "Machine code already exists."
            });
        }

        machine.MachineCode = updatedMachine.MachineCode;
        machine.MachineName = updatedMachine.MachineName;
        machine.MachineType = updatedMachine.MachineType;
        machine.MachineStatus = updatedMachine.MachineStatus;
        machine.Temperature = updatedMachine.Temperature;
        machine.MotorCurrent = updatedMachine.MotorCurrent;
        machine.Vibration = updatedMachine.Vibration;
        machine.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(machine);
    }

    // 5. Delete machine
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        var machine = await _context.Machines.FindAsync(id);

        if (machine == null)
        {
            return NotFound(new
            {
                Message = $"Machine with ID {id} not found"
            });
        }

        _context.Machines.Remove(machine);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Machine deleted successfully",
            MachineId = id
        });
    }
}