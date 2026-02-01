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
            if (dto == null || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Username and password are required");

            // ================= ADMIN LOGIN =================
            var admin = _db.Admins
                .FirstOrDefault(x => x.FullName == dto.Username);

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
                .FirstOrDefault(x => x.Terminal_ID == dto.Username);

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
                .FirstOrDefault(x => x.Beneficiary_ID == dto.Username);

            if (ben != null && BCrypt.Net.BCrypt.Verify(dto.Password, ben.Password))
            {
                return Ok(new
                {
                    token = _jwt.GenerateToken(ben.Beneficiary_ID, "Beneficiary"),
                    role = "Beneficiary"
                });
            }

            // ================= INVALID =================
            return Unauthorized("Invalid credentials");
        }
    }
}
