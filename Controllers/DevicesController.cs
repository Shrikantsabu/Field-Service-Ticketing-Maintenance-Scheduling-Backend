using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.DTOs;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class DevicesController : Controller
    {
        private readonly FieldOpsDbContext _context;
        public DevicesController(FieldOpsDbContext context)
        {
            _context = context;
        }
        // POST: api/v1/devices - Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDevice(CreateDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.DeviceType) ||
                string.IsNullOrWhiteSpace(request.SiteLocation))
            {
                return BadRequest(new { error = "All fields are required" });
            }
            var device = new Device
            {
                Name = request.Name,
                DeviceType = request.DeviceType,
                SiteLocation = request.SiteLocation,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            var response = new DeviceResponse
            {
                Id = device.Id,
                Name = device.Name,
                DeviceType = device.DeviceType,
                SiteLocation = device.SiteLocation,
                Status = device.Status,
                CreatedAt = device.CreatedAt
            };
            return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, response);
        }
        // GET: api/v1/devices - all authenticated users, paginated
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDevices([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;
            var devices = await _context.Devices
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var response = devices.Select(d => new DeviceResponse
            {
                Id = d.Id,
                Name = d.Name,
                DeviceType = d.DeviceType,
                SiteLocation = d.SiteLocation,
                Status = d.Status,
                CreatedAt = d.CreatedAt
            });
            return Ok(response);
        }
        // GET: api/v1/devices/{id} - any authenticated user
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
                return NotFound(new { error = "Device not found" });
            var response = new DeviceResponse
            {
                Id = device.Id,
                Name = device.Name,
                DeviceType = device.DeviceType,
                SiteLocation = device.SiteLocation,
                Status = device.Status,
                CreatedAt = device.CreatedAt
            };
            return Ok(response);
        }
    }
}


