using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs
{
    public class UpdateStatusRequest : Controller
    {
        public Status Status { get; set; }
    }
}
