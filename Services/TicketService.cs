using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;
using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services
{
    public class TicketService : Controller
    {
        private readonly FieldOpsDbContext _context;
        private const int TechnicianCapacityLimit = 5;
        public TicketService(FieldOpsDbContext context)
        {
            _context = context;
        }
        public DateTime ComputeSlaDueAt(Priority priority)
        {
            return priority switch
            {
                Priority.LOW => DateTime.UtcNow.AddDays(7),
                Priority.MEDIUM => DateTime.UtcNow.AddDays(3),
                Priority.HIGH => DateTime.UtcNow.AddDays(1),
                Priority.CRITICAL => DateTime.UtcNow.AddHours(12),
                _ => DateTime.UtcNow.AddDays(3)
            };
        }
        public async Task<bool> CanAssignToTechnicianAsync(int technicianId)
        {
            var activeTicketsCount = await _context.Tickets.CountAsync(t =>
                t.AssignedTechnicianId == technicianId &&
                (t.Status == Status.ASSIGNED || t.Status == Status.IN_PROGRESS));
            return activeTicketsCount < TechnicianCapacityLimit;
        }
        public async Task<List<Ticket>> EscalateOverdueTicketsAsync()
        {
            var now = DateTime.UtcNow;

            // Tickets not resolved or escalated, and past SLA due time
            var overdueTickets = await _context.Tickets
                .Where(t => (t.Status == Status.OPEN || t.Status == Status.ASSIGNED || t.Status == Status.IN_PROGRESS) &&
                            t.SlaDueAt <= now)
                .ToListAsync();

            foreach (var ticket in overdueTickets)
            {
                // Raise priority (capped at CRITICAL)
                ticket.Priority = ticket.Priority switch
                {
                    Priority.LOW => Priority.MEDIUM,
                    Priority.MEDIUM => Priority.HIGH,
                    Priority.HIGH => Priority.CRITICAL,
                    Priority.CRITICAL => Priority.CRITICAL,
                    _ => ticket.Priority
                };

                ticket.Status = Status.ESCALATED;
                ticket.EscalatedAt = now;

                // Optionally log or notify here
            }

            await _context.SaveChangesAsync();

            return overdueTickets;
        }

    }
}
