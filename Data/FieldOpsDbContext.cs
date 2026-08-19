using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data
{
    public class FieldOpsDbContext : DbContext
    {
        public FieldOpsDbContext(DbContextOptions<FieldOpsDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}
