using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs
{
    public class CreateTicketRequest : Controller
    {
        public string Description { get; set; } = null!;
        public Priority? PriorityHint { get; set; } // optional when fault is reported
    }
    public class TicketResponse
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Description { get; set; } = null!;
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime SlaDueAt { get; set; }
        public string WeatherRiskFlag { get; set; } = null!;
        public DateTime? EscalatedAt { get; set; }
    }
}
