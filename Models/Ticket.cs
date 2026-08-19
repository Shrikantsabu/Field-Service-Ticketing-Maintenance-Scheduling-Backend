using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models
{
    public class Ticket : Controller
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Priority Priority { get; set; }
        public Status Status { get; set; } = Status.OPEN;
        public int? AssignedTechnicianId { get; set; }
        public User? AssignedTechnician { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime SlaDueAt { get; set; }
        public string WeatherRiskFlag { get; set; } = "UNKNOWN";
        public DateTime? EscalatedAt { get; set; }
    }
}
