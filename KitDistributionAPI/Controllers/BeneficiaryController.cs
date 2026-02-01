using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitDistributionAPI.Data;
using System.Security.Claims;
using System.Linq;

namespace KitDistributionAPI.Controllers
{
    [Authorize(Roles = "Beneficiary")]
    [ApiController]
    [Route("api/beneficiary")]
    public class BeneficiaryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BeneficiaryController(AppDbContext db)
        {
            _db = db;
        }

        // ===============================
        // CURRENT DISTRIBUTION STATUS
        // ===============================
        [HttpGet("distribution-status")]
        public IActionResult GetCurrentStatus()
        {
            string beneficiaryId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(beneficiaryId))
                return Unauthorized("Invalid token");

            var latest = _db.KitTransactions
                .Where(x => x.Beneficiary_ID == beneficiaryId)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (latest == null)
            {
                return Ok(new
                {
                    month = "-",
                    amount = 0,
                    status = "Pending"
                });
            }

            return Ok(new
            {
                month = latest.Month,
                amount = latest.Amount,
                status = latest.Status
            });
        }

        // ===============================
        // DISTRIBUTION HISTORY
        // ===============================
        [HttpGet("history")]
        public IActionResult History()
        {
            string beneficiaryId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(beneficiaryId))
                return Unauthorized("Invalid token");

            var history = _db.KitTransactions
                .Where(x => x.Beneficiary_ID == beneficiaryId)
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    month = x.Month,
                    date = x.Date,
                    amount = x.Amount,
                    status = x.Status
                })
                .ToList();

            return Ok(history);
        }
    }
}
