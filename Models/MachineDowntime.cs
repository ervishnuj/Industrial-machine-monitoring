using System.Text.Json.Serialization;
namespace IndustrialMachineMonitoring.Models
{
    public class MachineDowntime
    {
        public int Id { get; set; }

        public int MachineId { get; set; }

        public DateTime DowntimeStart { get; set; }

        public DateTime? DowntimeEnd { get; set; }

        public string Reason { get; set; } = "Communication Lost";

        public bool IsResolved { get; set; }

        [JsonIgnore]
        public Machine? Machine { get; set; }
    }
}