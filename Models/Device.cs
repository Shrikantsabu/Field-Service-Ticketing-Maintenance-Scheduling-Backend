namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models
{
    public class Device : Controller
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string SiteLocation { get; set; } = null!;
        public string Status { get; set; } = "ACTIVE";  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
