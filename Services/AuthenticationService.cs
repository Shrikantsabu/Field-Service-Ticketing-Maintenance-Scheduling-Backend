using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services
{
    public class AuthenticationService
    {
        private readonly IConfiguration _config;
        public AuthenticationService(IConfiguration config)
        {
            _config = config;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string password, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashed);
        }
        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
