
// using IndustrialMachineMonitoring.Data;
// using IndustrialMachineMonitoring.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;


// namespace IndustrialMachineMonitoring.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class SensorReadingsController : ControllerBase
// {
//     private readonly MachineDbContext _context;

//     public SensorReadingsController(MachineDbContext context)
//     {
//         _context = context;
//     }

//     // POST: api/SensorReadings
//     [HttpPost]
//     public async Task<IActionResult> CreateSensorReading(SensorReading reading)
//     {
//         var machineExists = await _context.Machines
//             .AnyAsync(m => m.Id == reading.MachineId);

//         if (!machineExists)
//         {
//             return NotFound(new
//             {
//                 Message = $"Machine ID {reading.MachineId} not found."
//             });
//         }

//         reading.RecordedAt = DateTime.UtcNow;

//         // _context.SensorReadings.Add(reading);
//         // await _context.SaveChangesAsync();

//         // return CreatedAtAction(
//         //     nameof(GetSensorReadingById),
//         //     new { id = reading.Id },
//         //     reading
//         // );

//         // POST: api/SensorReadings
//         [HttpPost]
//         public async Task<IActionResult> CreateSensorReading(SensorReading reading)
//         {
//             var machineExists = await _context.Machines
//                 .AnyAsync(m => m.Id == reading.MachineId);

//             if (!machineExists)
//             {
//                 return NotFound(new
//                 {
//                     Message = $"Machine ID {reading.MachineId} not found."
//                 });
//             }

//             // Get previous sensor reading before saving the new one
//             var previousReading = await _context.SensorReadings
//                 .Where(r => r.MachineId == reading.MachineId)
//                 .OrderByDescending(r => r.RecordedAt)
//                 .FirstOrDefaultAsync();

//             reading.RecordedAt = DateTime.UtcNow;

//             // Save sensor reading
//             _context.SensorReadings.Add(reading);
//             await _context.SaveChangesAsync();

//             // Temperature alert: trigger only when crossing the threshold
//             if (reading.Temperature > 80 &&
//                 (previousReading == null || previousReading.Temperature <= 80))
//             {
//                 var severity = reading.Temperature > 100
//                     ? "CRITICAL"
//                     : "HIGH";

//                 _context.AlertLogs.Add(new AlertLog
//                 {
//                     MachineId = reading.MachineId,
//                     AlertType = "TEMPERATURE",
//                     Severity = severity,
//                     Message = $"High temperature detected: {reading.Temperature} °C",
//                     ActualValue = reading.Temperature,
//                     ThresholdValue = 80,
//                     CreatedAt = DateTime.UtcNow
//                 });
//             }

//             // Motor current alert
//             if (reading.MotorCurrent > 15 &&
//                 (previousReading == null || previousReading.MotorCurrent <= 15))
//             {
//                 var severity = reading.MotorCurrent > 20
//                     ? "CRITICAL"
//                     : "HIGH";

//                 _context.AlertLogs.Add(new AlertLog
//                 {
//                     MachineId = reading.MachineId,
//                     AlertType = "MOTOR_CURRENT",
//                     Severity = severity,
//                     Message = $"High motor current detected: {reading.MotorCurrent} A",
//                     ActualValue = reading.MotorCurrent,
//                     ThresholdValue = 15,
//                     CreatedAt = DateTime.UtcNow
//                 });
//             }

//             // Vibration alert
//             if (reading.Vibration > 5 &&
//                 (previousReading == null || previousReading.Vibration <= 5))
//             {
//                 var severity = reading.Vibration > 8
//                     ? "CRITICAL"
//                     : "HIGH";

//                 _context.AlertLogs.Add(new AlertLog
//                 {
//                     MachineId = reading.MachineId,
//                     AlertType = "VIBRATION",
//                     Severity = severity,
//                     Message = $"High vibration detected: {reading.Vibration}",
//                     ActualValue = reading.Vibration,
//                     ThresholdValue = 5,
//                     CreatedAt = DateTime.UtcNow
//                 });
//             }

//             // Save generated alerts
//             await _context.SaveChangesAsync();

//             return CreatedAtAction(
//                 nameof(GetSensorReadingById),
//                 new { id = reading.Id },
//                 reading
//             );
//         }
//     }

//     // GET: api/SensorReadings
//     [HttpGet]
//     public async Task<IActionResult> GetAllSensorReadings()
//     {
//         var readings = await _context.SensorReadings
//             .OrderByDescending(r => r.RecordedAt)
//             .ToListAsync();

//         return Ok(readings);
//     }

//     // GET: api/SensorReadings/1
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetSensorReadingById(int id)
//     {
//         var reading = await _context.SensorReadings
//             .FindAsync(id);

//         if (reading == null)
//         {
//             return NotFound(new
//             {
//                 Message = $"Sensor reading {id} not found."
//             });
//         }

//         return Ok(reading);
//     }

//     // GET: api/SensorReadings/machine/1
//     [HttpGet("machine/{machineId}")]
//     public async Task<IActionResult> GetReadingsByMachine(int machineId)
//     {
//         var readings = await _context.SensorReadings
//             .Where(r => r.MachineId == machineId)
//             .OrderByDescending(r => r.RecordedAt)
//             .ToListAsync();

//         return Ok(readings);
//     }
// }


using IndustrialMachineMonitoring.Data;
using IndustrialMachineMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorReadingsController : ControllerBase
{
    private readonly MachineDbContext _context;

    public SensorReadingsController(MachineDbContext context)
    {
        _context = context;
    }

    // POST: api/SensorReadings
    [HttpPost]
    public async Task<IActionResult> CreateSensorReading(SensorReading reading)
    {
        var machineExists = await _context.Machines
            .AnyAsync(m => m.Id == reading.MachineId);

        if (!machineExists)
        {
            return NotFound(new
            {
                Message = $"Machine ID {reading.MachineId} not found."
            });
        }

        var previousReading = await _context.SensorReadings
            .Where(r => r.MachineId == reading.MachineId)
            .OrderByDescending(r => r.RecordedAt)
            .FirstOrDefaultAsync();

        reading.RecordedAt = DateTime.UtcNow;

        // Downtime recovery detection
        if (previousReading != null)
        {
            var timeGap = reading.RecordedAt - previousReading.RecordedAt;

            if (timeGap.TotalSeconds > 15)
            {
                var openDowntime = await _context.MachineDowntimes
                    .Where(d => d.MachineId == reading.MachineId && !d.IsResolved)
                    .OrderByDescending(d => d.DowntimeStart)
                    .FirstOrDefaultAsync();

                if (openDowntime != null)
                {
                    // Close existing downtime
                    openDowntime.DowntimeEnd = reading.RecordedAt;
                    openDowntime.IsResolved = true;
                }
                else
                {
                    // Create downtime record for the detected communication gap
                    _context.MachineDowntimes.Add(new MachineDowntime
                    {
                        MachineId = reading.MachineId,
                        DowntimeStart = previousReading.RecordedAt.AddSeconds(15),
                        DowntimeEnd = reading.RecordedAt,
                        Reason = "Communication Lost",
                        IsResolved = true
                    });
                }
            }
        }

        reading.RecordedAt = DateTime.UtcNow;

        _context.SensorReadings.Add(reading);
        await _context.SaveChangesAsync();

        // Temperature alert
        if (reading.Temperature > 80 &&
            (previousReading == null || previousReading.Temperature <= 80))
        {
            var severity = reading.Temperature > 100
                ? "CRITICAL"
                : "HIGH";

            _context.AlertLogs.Add(new AlertLog
            {
                MachineId = reading.MachineId,
                AlertType = "TEMPERATURE",
                Severity = severity,
                Message = $"High temperature detected: {reading.Temperature} °C",
                ActualValue = reading.Temperature,
                ThresholdValue = 80,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Motor current alert
        if (reading.MotorCurrent > 15 &&
            (previousReading == null || previousReading.MotorCurrent <= 15))
        {
            var severity = reading.MotorCurrent > 20
                ? "CRITICAL"
                : "HIGH";

            _context.AlertLogs.Add(new AlertLog
            {
                MachineId = reading.MachineId,
                AlertType = "MOTOR_CURRENT",
                Severity = severity,
                Message = $"High motor current detected: {reading.MotorCurrent} A",
                ActualValue = reading.MotorCurrent,
                ThresholdValue = 15,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Vibration alert
        if (reading.Vibration > 5 &&
            (previousReading == null || previousReading.Vibration <= 5))
        {
            var severity = reading.Vibration > 8
                ? "CRITICAL"
                : "HIGH";

            _context.AlertLogs.Add(new AlertLog
            {
                MachineId = reading.MachineId,
                AlertType = "VIBRATION",
                Severity = severity,
                Message = $"High vibration detected: {reading.Vibration}",
                ActualValue = reading.Vibration,
                ThresholdValue = 5,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSensorReadingById),
            new { id = reading.Id },
            reading
        );
    }

    // GET: api/SensorReadings
    [HttpGet]
    public async Task<IActionResult> GetAllSensorReadings()
    {
        var readings = await _context.SensorReadings
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();

        return Ok(readings);
    }

    // GET: api/SensorReadings/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSensorReadingById(int id)
    {
        var reading = await _context.SensorReadings.FindAsync(id);

        if (reading == null)
        {
            return NotFound(new
            {
                Message = $"Sensor reading {id} not found."
            });
        }

        return Ok(reading);
    }

    // GET: api/SensorReadings/machine/1
    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetReadingsByMachine(int machineId)
    {
        var machineExists = await _context.Machines
            .AnyAsync(m => m.Id == machineId);

        if (!machineExists)
        {
            return NotFound(new
            {
                Message = $"Machine ID {machineId} not found."
            });
        }

        var readings = await _context.SensorReadings
            .Where(r => r.MachineId == machineId)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();

        return Ok(readings);
    }
}
