using Microsoft.AspNetCore.Mvc;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models
{
    public enum Role
    {
        Admin,
        Technician
    }
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
