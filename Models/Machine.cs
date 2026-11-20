using System.ComponentModel.DataAnnotations;

namespace IndustrialMachineMonitoring.Models;

public class Machine
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string MachineCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MachineName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MachineType { get; set; }

    [MaxLength(20)]
    public string MachineStatus { get; set; } = "STOPPED";

    public double? Temperature { get; set; }

    public double? MotorCurrent { get; set; }

    public double? Vibration { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public ICollection<SensorReading> SensorReadings { get; set; }
    = new List<SensorReading>();
    public ICollection<AlertLog> AlertLogs { get; set; } = new List<AlertLog>();
}