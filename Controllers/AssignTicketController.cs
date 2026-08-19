using Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs;
using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Controllers
{
    public class AssignTicketController : Controller
    {
        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound(new { error = "Ticket not found" });

            var technician = await _context.Users
                .Where(u => u.Id == request.TechnicianId && u.Role == Role.Technician)
                .FirstOrDefaultAsync();

            if (technician == null)
                return NotFound(new { error = "Technician not found" });

            var canAssign = await _ticketService.CanAssignToTechnicianAsync(request.TechnicianId);
            if (!canAssign)
                return Conflict(new { error = "Technician capacity exceeded" });

            ticket.AssignedTechnicianId = technician.Id;
            ticket.Status = Status.ASSIGNED;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ticket assigned" });
        }

    }
}
