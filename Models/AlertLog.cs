// using System.ComponentModel.DataAnnotations;

// namespace IndustrialMachineMonitoring.Models;

// public class AlertLog
// {
//     public int Id { get; set; }

//     public int MachineId { get; set; }

//     [Required]
//     [MaxLength(50)]
//     public string AlertType { get; set; } = string.Empty;

//     [Required]
//     [MaxLength(20)]
//     public string Severity { get; set; } = "MEDIUM";

//     [Required]
//     [MaxLength(250)]
//     public string Message { get; set; } = string.Empty;

//     public double ActualValue { get; set; }

//     public double ThresholdValue { get; set; }

//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//     public bool IsAcknowledged { get; set; } = false;

//     public DateTime? AcknowledgedAt { get; set; }

//     public Machine? Machine { get; set; }
// }

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IndustrialMachineMonitoring.Models;

public class AlertLog
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AlertType { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = "MEDIUM";

    [Required]
    [MaxLength(250)]
    public string Message { get; set; } = string.Empty;

    public double ActualValue { get; set; }

    public double ThresholdValue { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAcknowledged { get; set; } = false;

    public DateTime? AcknowledgedAt { get; set; }

    [JsonIgnore]
    public Machine? Machine { get; set; }
}