using IndustrialMachineMonitoring.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineDowntimesController : ControllerBase
    {
        private readonly MachineDbContext _context;

        public MachineDowntimesController(MachineDbContext context)
        {
            _context = context;
        }

        // Get all downtime records
        // [HttpGet]
        // public async Task<IActionResult> GetAllDowntimes()
        // {
        //     var downtimes = await _context.MachineDowntimes
        //         .Include(d => d.Machine)
        //         .OrderByDescending(d => d.DowntimeStart)
        //         .Select(d => new
        //         {
        //             d.Id,
        //             d.MachineId,
        //             MachineName = d.Machine.MachineName,
        //             d.DowntimeStart,
        //             d.DowntimeEnd,
        //             d.Reason,
        //             d.IsResolved,

        //             DurationMinutes = d.DowntimeEnd.HasValue
        //                 ? (d.DowntimeEnd.Value - d.DowntimeStart).TotalMinutes
        //                 : (DateTime.UtcNow - d.DowntimeStart).TotalMinutes
        //         })
        //         .ToListAsync();

        //     return Ok(downtimes);
        // }

        [HttpGet]
        public async Task<IActionResult> GetAllDowntimes()
        {
            var records = await _context.MachineDowntimes
                .Include(d => d.Machine)
                .OrderByDescending(d => d.DowntimeStart)
                .ToListAsync();

            var downtimes = records.Select(d => new
            {
                d.Id,
                d.MachineId,
                MachineName = d.Machine?.MachineName,
                d.DowntimeStart,
                d.DowntimeEnd,
                d.Reason,
                d.IsResolved,

                DurationMinutes = (
                    (d.DowntimeEnd ?? DateTime.UtcNow) - d.DowntimeStart
                ).TotalMinutes
            });

            return Ok(downtimes);
        }

        // Get downtime by ID
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetDowntimeById(int id)
        // {
        //     var downtime = await _context.MachineDowntimes
        //         .Include(d => d.Machine)
        //         .Where(d => d.Id == id)
        //         .Select(d => new
        //         {
        //             d.Id,
        //             d.MachineId,
        //             MachineName = d.Machine.MachineName,
        //             d.DowntimeStart,
        //             d.DowntimeEnd,
        //             d.Reason,
        //             d.IsResolved
        //         })
        //         .FirstOrDefaultAsync();

        //     if (downtime == null)
        //     {
        //         return NotFound(new
        //         {
        //             message = "Downtime record not found"
        //         });
        //     }

        //     return Ok(downtime);
        // }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDowntimeById(int id)
        {
            var d = await _context.MachineDowntimes
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d == null)
            {
                return NotFound(new
                {
                    message = "Downtime record not found"
                });
            }

            var downtime = new
            {
                d.Id,
                d.MachineId,
                MachineName = d.Machine?.MachineName,
                d.DowntimeStart,
                d.DowntimeEnd,
                d.Reason,
                d.IsResolved,

                DurationMinutes = (
                    (d.DowntimeEnd ?? DateTime.UtcNow) - d.DowntimeStart
                ).TotalMinutes
            };

            return Ok(downtime);
        }

        // Get downtime records for a specific machine
        // [HttpGet("machine/{machineId}")]
        // public async Task<IActionResult> GetMachineDowntimes(int machineId)
        // {
        //     var downtimes = await _context.MachineDowntimes
        //         .Where(d => d.MachineId == machineId)
        //         .OrderByDescending(d => d.DowntimeStart)
        //         .Select(d => new
        //         {
        //             d.Id,
        //             d.MachineId,
        //             d.DowntimeStart,
        //             d.DowntimeEnd,
        //             d.Reason,
        //             d.IsResolved,

        //             DurationMinutes = d.DowntimeEnd.HasValue
        //                 ? (d.DowntimeEnd.Value - d.DowntimeStart).TotalMinutes
        //                 : (DateTime.UtcNow - d.DowntimeStart).TotalMinutes
        //         })
        //         .ToListAsync();

        //     return Ok(downtimes);
        // }

        [HttpGet("machine/{machineId}")]
        public async Task<IActionResult> GetMachineDowntimes(int machineId)
        {
            var records = await _context.MachineDowntimes
                .Where(d => d.MachineId == machineId)
                .OrderByDescending(d => d.DowntimeStart)
                .ToListAsync();

            var downtimes = records.Select(d => new
            {
                d.Id,
                d.MachineId,
                d.DowntimeStart,
                d.DowntimeEnd,
                d.Reason,
                d.IsResolved,

                DurationMinutes = (
                    (d.DowntimeEnd ?? DateTime.UtcNow) - d.DowntimeStart
                ).TotalMinutes
            });

            return Ok(downtimes);
        }
    }
}