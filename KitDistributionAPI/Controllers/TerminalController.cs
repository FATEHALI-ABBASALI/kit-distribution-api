using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitDistributionAPI.Data;
using KitDistributionAPI.Models;
using KitDistributionAPI.DTOs;
using System.Security.Claims;
using System.Linq;
using System;

namespace KitDistributionAPI.Controllers
{
    [Authorize(Roles = "Terminal")]
    [ApiController]
    [Route("api/terminal")]
    public class TerminalController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TerminalController(AppDbContext db)
        {
            _db = db;
        }

        // ==========================================
        // DISTRIBUTE KIT (AUTO MONTH FROM DATE)
        // ==========================================
        [HttpPost("distribute-kit")]
        public IActionResult DistributeKit([FromBody] DistributeKitDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Beneficiary_ID))
                return BadRequest("Beneficiary ID is required");

            // ✅ Terminal ID from JWT
            string terminalId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(terminalId))
                return Unauthorized("Invalid terminal token");

            // ✅ Use server date if client date missing
            DateTime distributionDate =
                dto.Date == DateTime.MinValue ? DateTime.Today : dto.Date;

            string month = distributionDate.ToString("MMMM");

            // ✅ Verify beneficiary exists
            bool beneficiaryExists = _db.Beneficiaries
                .Any(b => b.Beneficiary_ID == dto.Beneficiary_ID);

            if (!beneficiaryExists)
                return NotFound("Beneficiary not found");

            // ✅ One kit per beneficiary per month per year
            bool alreadyDistributed = _db.KitTransactions.Any(x =>
                x.Beneficiary_ID == dto.Beneficiary_ID &&
                x.Month == month &&
                x.Date.Year == distributionDate.Year
            );

            if (alreadyDistributed)
                return BadRequest("Kit already distributed for this month");

            // ✅ Save transaction
            var transaction = new KitTransaction
            {
                Beneficiary_ID = dto.Beneficiary_ID,
                Terminal_ID = terminalId,
                Month = month,
                Date = distributionDate,
                Amount = 100,
                Status = "Received"
            };

            _db.KitTransactions.Add(transaction);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Kit distributed successfully",
                beneficiaryId = dto.Beneficiary_ID,
                terminalId,
                month,
                date = distributionDate.ToShortDateString(),
                amount = 100,
                status = "Received"
            });
        }
    }
}
