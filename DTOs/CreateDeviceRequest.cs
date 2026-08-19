namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs
{
    public class CreateDeviceRequest : Controller
    {
        public string Name { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string SiteLocation { get; set; } = null!;
    }

    public class DeviceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string SiteLocation { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
