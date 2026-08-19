using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly FieldOpsDbContext _context;
        private readonly AuthenticationService _authService;
        public AuthController(FieldOpsDbContext context, AuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!Enum.TryParse<Role>(request.Role, true, out var role))
            {
                return BadRequest(new { error = "Invalid role" });
            }
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return Conflict(new { error = "Email already registered" });
            var user = new User
            {
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
    // DTOs for requests
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

