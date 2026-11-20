using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IndustrialMachineMonitoring.Models;





public class SensorReading
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public double Temperature { get; set; }

    public double MotorCurrent { get; set; }

    public double Vibration { get; set; }

    public DateTime RecordedAt { get; set; }
        = DateTime.UtcNow;

    [JsonIgnore]
    public Machine? Machine { get; set; }



}