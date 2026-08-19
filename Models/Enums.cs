namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models
{
    public class Enums : Controller
    {
        public enum Priority
        {
            LOW,
            MEDIUM,
            HIGH,
            CRITICAL
        }
        public enum Status
        {
            OPEN,
            ASSIGNED,
            IN_PROGRESS,
            RESOLVED,
            ESCALATED
        }
    }
}
