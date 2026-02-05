using Microsoft.AspNetCore.Mvc;
using KitDistributionAPI.Data;
using KitDistributionAPI.DTOs;
using KitDistributionAPI.Services;
using BCrypt.Net;

namespace KitDistributionAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // =====================================================
        // LOGIN (ADMIN / TERMINAL / BENEFICIARY)
        // =====================================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            string username = dto.Username.Trim();

            // ================= ADMIN LOGIN =================
            var admin = _db.Admins
                .FirstOrDefault(x => x.FullName.ToLower() == username.ToLower());

            if (admin != null && BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            {
                return Ok(new
                {
                    token = _jwt.GenerateToken(admin.Admin_ID.ToString(), "Admin"),
                    role = "Admin"
                });
            }

            // ================= TERMINAL LOGIN =================
            var terminal = _db.TerminalUsers
                .FirstOrDefault(x => x.Terminal_ID == username);

            if (terminal != null && BCrypt.Net.BCrypt.Verify(dto.Password, terminal.Password))
            {
                return Ok(new
                {
                    token = _jwt.GenerateToken(terminal.Terminal_ID, "Terminal"),
                    role = "Terminal"
                });
            }

            // ================= BENEFICIARY LOGIN =================
            var ben = _db.Beneficiaries
                .FirstOrDefault(x => x.Beneficiary_ID == username);

            if (ben != null && BCrypt.Net.BCrypt.Verify(dto.Password, ben.Password))
            {
                return Ok(new
                {
                    token = _jwt.GenerateToken(ben.Beneficiary_ID, "Beneficiary"),
                    role = "Beneficiary"
                });
            }

            // ================= INVALID =================
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}
