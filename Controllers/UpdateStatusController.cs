using Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;
using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Controllers
{
    public class UpdateStatusController : Controller
    {
        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var userId = int.Parse(User.FindFirst("nameid")?.Value!);
            var userRole = User.FindFirst("role")?.Value;

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound(new { error = "Ticket not found" });

            if (userRole == "Technician" && ticket.AssignedTechnicianId != userId)
                return Forbid();

            // Validate legal status transitions (example)
            bool validTransition = request.Status switch
            {
                Status.ASSIGNED => ticket.Status == Status.OPEN,
                Status.IN_PROGRESS => ticket.Status == Status.ASSIGNED,
                Status.RESOLVED => ticket.Status == Status.IN_PROGRESS,
                Status.ESCALATED => true,
                _ => false
            };

            if (!validTransition)
                return Conflict(new { error = "Illegal status transition" });

            ticket.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated" });
        }

    }
}
