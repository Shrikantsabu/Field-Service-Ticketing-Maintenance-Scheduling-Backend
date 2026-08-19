using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using static Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models.Enums;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketsController : Controller
    {
        private readonly FieldOpsDbContext _context;
        private readonly TicketService _ticketService;
        private readonly WeatherService _weatherService;
        public TicketsController(FieldOpsDbContext context, TicketService ticketService, WeatherService weatherService)
        {
            _context = context;
            _ticketService = ticketService;
            _weatherService = weatherService;
        }

        // POST /api/v1/devices/{id}/fault - create ticket from device fault (authenticated users)
        [HttpPost("/api/v1/devices/{deviceId}/fault")]
        [Authorize]
        public async Task<IActionResult> ReportFault(int deviceId, [FromBody] CreateTicketRequest request)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
                return NotFound(new { error = "Device not found" });
            var priority = request.PriorityHint ?? Priority.MEDIUM;  // default if not provided
            var slaDue = _ticketService.ComputeSlaDueAt(priority);
            var ticket = new Ticket
            {
                DeviceId = deviceId,
                Description = request.Description,
                Priority = priority,
                Status = Status.OPEN,
                CreatedAt = DateTime.UtcNow,
                SlaDueAt = slaDue,
                WeatherRiskFlag = "UNKNOWN"
            };
            var riskFlag = await _weatherService.GetWeatherRiskFlagAsync(device.SiteLocation);
            // Update ticket with weather risk flag
            ticket.WeatherRiskFlag = riskFlag;
            await _context.SaveChangesAsync();

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            var response = new TicketResponse
            {
                Id = ticket.Id,
                DeviceId = ticket.DeviceId,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                CreatedAt = ticket.CreatedAt,
                SlaDueAt = ticket.SlaDueAt,
                WeatherRiskFlag = ticket.WeatherRiskFlag,
                EscalatedAt = ticket.EscalatedAt
            };
            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, response);
        }
        // GET /api/v1/tickets/{id} - Admin sees any ticket; Technician only theirs
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var userId = int.Parse(User.FindFirst("nameid")?.Value!);
            var userRole = User.FindFirst("role")?.Value;
            var ticket = await _context.Tickets
                .Include(t => t.Device)
                .SingleOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
                return NotFound(new { error = "Ticket not found" });
            if (userRole == "Technician" && ticket.AssignedTechnicianId != userId)
                return Forbid();
            var response = new TicketResponse
            {
                Id = ticket.Id,
                DeviceId = ticket.DeviceId,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                CreatedAt = ticket.CreatedAt,
                SlaDueAt = ticket.SlaDueAt,
                WeatherRiskFlag = ticket.WeatherRiskFlag,
                EscalatedAt = ticket.EscalatedAt
            };
            return Ok(response);
        }
        // POST /api/v1/tickets/escalate - Admin Only
        [HttpPost("escalate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EscalateTickets()
        {
            var escalated = await _ticketService.EscalateOverdueTicketsAsync();
            return Ok(new { message = $"{escalated.Count} tickets escalated" });
        }
        // GET /api/v1/tickets
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTickets(
            [FromQuery] Status? status = null,
            [FromQuery] Priority? priority = null,
            [FromQuery] int? technicianId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var userId = int.Parse(User.FindFirst("nameid")?.Value!);
            var userRole = User.FindFirst("role")?.Value;

            var query = _context.Tickets
                .Include(t => t.Device)
                .AsQueryable();

            // Role-based filtering
            if (userRole == "Technician")
            {
                query = query.Where(t => t.AssignedTechnicianId == userId);
            }

            // Apply filters
            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (technicianId.HasValue)
                query = query.Where(t => t.AssignedTechnicianId == technicianId.Value);

            var totalCount = await query.CountAsync();

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = tickets.Select(t => new TicketResponse
            {
                Id = t.Id,
                DeviceId = t.DeviceId,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                AssignedTechnicianId = t.AssignedTechnicianId,
                CreatedAt = t.CreatedAt,
                SlaDueAt = t.SlaDueAt,
                WeatherRiskFlag = t.WeatherRiskFlag,
                EscalatedAt = t.EscalatedAt
            });

            return Ok(new
            {
                total = totalCount,
                page,
                pageSize,
                tickets = result
            });
        }

    }
}
