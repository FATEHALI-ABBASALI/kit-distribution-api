using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitDistributionAPI.Data;
using KitDistributionAPI.DTOs;
using KitDistributionAPI.Models;
using KitDistributionAPI.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;

namespace KitDistributionAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly QrCodeService _qr;

        public AdminController(AppDbContext db, QrCodeService qr)
        {
            _db = db;
            _qr = qr;

            // ✅ Required for QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ==========================================
        // ADD BENEFICIARY
        // ==========================================
        [HttpPost("beneficiary")]
        public IActionResult AddBeneficiary([FromBody] BeneficiaryCreateDto dto)
        {
            string beneficiaryId = "BEN" + Guid.NewGuid().ToString("N")[..6];
            string password = Guid.NewGuid().ToString("N")[..8];

            string cardId = _qr.GenerateQrCode(
                beneficiaryId,
                dto.FullName,
                dto.State_City
            );

            var beneficiary = new Beneficiary
            {
                Beneficiary_ID = beneficiaryId,
                Card_ID = cardId,
                FullName = dto.FullName,
                Mobile = dto.Mobile,
                State_City = dto.State_City,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Status = "Active",
                Create_Date = DateTime.Now
            };

            _db.Beneficiaries.Add(beneficiary);
            _db.SaveChanges();

            return Ok(new
            {
                beneficiaryId,
                password,   // ⚠ shown ONCE
                cardId
            });
        }

        // ==========================================
        // ADD TERMINAL USER
        // ==========================================
        [HttpPost("terminal-user")]
        public IActionResult AddTerminalUser([FromQuery] string fullName)
        {
            string terminalId = "TERM" + Guid.NewGuid().ToString("N")[..6];
            string password = Guid.NewGuid().ToString("N")[..8];

            var terminal = new TerminalUser
            {
                Terminal_ID = terminalId,
                FullName = fullName,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Status = "Active",
                Create_Date = DateTime.Now
            };

            _db.TerminalUsers.Add(terminal);
            _db.SaveChanges();

            return Ok(new
            {
                terminalId,
                password // ⚠ shown ONCE
            });
        }

        // ==========================================
        // GET ALL BENEFICIARIES
        // ==========================================
        [HttpGet("beneficiaries")]
        public IActionResult GetBeneficiaries()
        {
            return Ok(_db.Beneficiaries
                .Select(b => new
                {
                    b.Beneficiary_ID,
                    b.FullName,
                    b.Mobile,
                    b.State_City,
                    b.Status
                })
                .ToList());
        }

        // ==========================================
        // GET ALL TERMINAL USERS
        // ==========================================
        [HttpGet("terminal-users")]
        public IActionResult GetTerminalUsers()
        {
            return Ok(_db.TerminalUsers
                .Select(t => new
                {
                    t.Terminal_ID,
                    t.FullName,
                    t.Status
                })
                .ToList());
        }

        // ==========================================
        // VIEW ALL TRANSACTIONS
        // ==========================================
        [HttpGet("transactions")]
        public IActionResult GetTransactions()
        {
            return Ok(_db.KitTransactions.ToList());
        }

        // ==========================================
        // REPORT BY DATE
        // ==========================================
        [HttpGet("report/{date}")]
        public IActionResult ReportByDate(DateTime date)
        {
            var data = _db.KitTransactions
                .Where(x => x.Date.Date == date.Date)
                .ToList();

            return Ok(data);
        }

        // ==========================================
        // EXPORT EXCEL REPORT
        // ==========================================
        [HttpGet("report/excel")]
        public IActionResult ExportExcel()
        {
            var data = _db.KitTransactions.ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Kit Report");

            ws.Cell(1, 1).Value = "Beneficiary ID";
            ws.Cell(1, 2).Value = "Terminal ID";
            ws.Cell(1, 3).Value = "Month";
            ws.Cell(1, 4).Value = "Amount";
            ws.Cell(1, 5).Value = "Date";
            ws.Cell(1, 6).Value = "Status";

            int row = 2;
            foreach (var x in data)
            {
                ws.Cell(row, 1).Value = x.Beneficiary_ID;
                ws.Cell(row, 2).Value = x.Terminal_ID;
                ws.Cell(row, 3).Value = x.Month;
                ws.Cell(row, 4).Value = x.Amount;
                ws.Cell(row, 5).Value = x.Date.ToShortDateString();
                ws.Cell(row, 6).Value = x.Status;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "KitReport.xlsx"
            );
        }

        // ==========================================
        // EXPORT PDF REPORT (PROFESSIONAL TABLE + TOTALS)
        // ==========================================
        [HttpGet("report/pdf")]
        public IActionResult ExportPdf(DateTime? from, DateTime? to)
        {
            var query = _db.KitTransactions.AsQueryable();

            if (from.HasValue && to.HasValue)
            {
                query = query.Where(x =>
                    x.Date.Date >= from.Value.Date &&
                    x.Date.Date <= to.Value.Date
                );
            }

            var data = query
                .Select(x => new
                {
                    x.Beneficiary_ID,
                    x.Terminal_ID,
                    x.Month,
                    Amount = x.Amount,   // ensure int
                    x.Status
                })
                .ToList();

            int totalRecords = data.Count;
            int totalAmount = data.Sum(x => x.Amount);

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        // TITLE
                        col.Item()
                            .AlignCenter()
                            .Text("Kit Distribution Report")
                            .FontSize(20)
                            .Bold();

                        col.Item().Height(10);

                        // DATE RANGE
                        col.Item().AlignCenter().Text(
                            from.HasValue && to.HasValue
                                ? $"From {from:dd-MM-yyyy} To {to:dd-MM-yyyy}"
                                : "All Records"
                        );

                        col.Item().Height(15);

                        // SUMMARY
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Total Records: {totalRecords}").Bold();
                            row.RelativeItem().AlignRight().Text($"Total Amount: ₹{totalAmount}").Bold();
                        });

                        col.Item().Height(15);

                        // TABLE
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });

                            // HEADER
                            table.Header(header =>
                            {
                                header.Cell().Text("Beneficiary ID").Bold();
                                header.Cell().Text("Terminal ID").Bold();
                                header.Cell().Text("Month").Bold();
                                header.Cell().Text("Amount").Bold();
                                header.Cell().Text("Status").Bold();
                            });

                            // ROWS
                            foreach (var x in data)
                            {
                                table.Cell().Text(x.Beneficiary_ID);
                                table.Cell().Text(x.Terminal_ID);
                                table.Cell().Text(x.Month);
                                table.Cell().Text($"₹{x.Amount}");
                                table.Cell().Text(x.Status);
                            }
                        });
                    });
                });
            });

            var bytes = pdf.GeneratePdf();
            return File(bytes, "application/pdf", "KitReport.pdf");
        }

        // ==========================================
        // RESET TERMINAL PASSWORD
        // ==========================================
        [HttpPost("reset-terminal-password")]
        public IActionResult ResetTerminalPassword([FromBody] ResetPasswordDto dto)
        {
            var terminal = _db.TerminalUsers
                .FirstOrDefault(x => x.Terminal_ID == dto.UserId);

            if (terminal == null)
                return NotFound("Terminal user not found");

            string newPassword = Guid.NewGuid().ToString("N")[..8];

            terminal.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            terminal.Update_Date = DateTime.Now;

            _db.SaveChanges();

            return Ok(new
            {
                terminalId = terminal.Terminal_ID,
                newPassword // ⚠ shown ONCE
            });
        }

        // ==========================================
        // RESET BENEFICIARY PASSWORD
        // ==========================================
        [HttpPost("reset-beneficiary-password")]
        public IActionResult ResetBeneficiaryPassword([FromBody] ResetPasswordDto dto)
        {
            var ben = _db.Beneficiaries
                .FirstOrDefault(x => x.Beneficiary_ID == dto.UserId);

            if (ben == null)
                return NotFound("Beneficiary not found");

            string newPassword = Guid.NewGuid().ToString("N")[..8];

            ben.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            ben.UpdateDate = DateTime.Now;

            _db.SaveChanges();

            return Ok(new
            {
                beneficiaryId = ben.Beneficiary_ID,
                newPassword // ⚠ shown ONCE
            });
        }
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(new
            {
                beneficiaries = _db.Beneficiaries
                    .Select(b => new {
                        b.Beneficiary_ID,
                        b.FullName,
                        b.Mobile,
                        b.State_City,
                        b.Status
                    }).ToList(),

                terminals = _db.TerminalUsers
                    .Select(t => new {
                        t.Terminal_ID,
                        t.FullName,
                        t.Status
                    }).ToList()
            });
        }
        // ==========================================
        // ADMIN DASHBOARD STATS
        // ==========================================
        [HttpGet("dashboard-stats")]
        public IActionResult DashboardStats()
        {
            var totalBeneficiaries = _db.Beneficiaries.Count();
            var totalTerminals = _db.TerminalUsers.Count();

            var totalKits = _db.KitTransactions.Count();
            var received = _db.KitTransactions.Count(x => x.Status == "Received");
            var pending = totalKits - received;

            return Ok(new
            {
                totalUsers = totalBeneficiaries + totalTerminals,
                totalBeneficiaries,
                totalTerminals,
                totalKits,
                received,
                pending
            });
        }

        // ==========================================
        // EDIT BENEFICIARY
        // ==========================================
        [HttpPut("beneficiary/{id}")]
        public IActionResult EditBeneficiary(string id, [FromBody] BeneficiaryCreateDto dto)
        {
            var ben = _db.Beneficiaries.FirstOrDefault(x => x.Beneficiary_ID == id);
            if (ben == null) return NotFound();

            ben.FullName = dto.FullName;
            ben.Mobile = dto.Mobile;
            ben.State_City = dto.State_City;
            ben.UpdateDate = DateTime.Now;

            _db.SaveChanges();
            return Ok("Beneficiary updated");
        }

        // ==========================================
        // DELETE BENEFICIARY
        // ==========================================
        [HttpDelete("beneficiary/{id}")]
        public IActionResult DeleteBeneficiary(string id)
        {
            var ben = _db.Beneficiaries.FirstOrDefault(x => x.Beneficiary_ID == id);
            if (ben == null) return NotFound();

            _db.Beneficiaries.Remove(ben);
            _db.SaveChanges();

            return Ok("Beneficiary deleted");
        }

        // ==========================================
        // EDIT TERMINAL USER
        // ==========================================
        [HttpPut("terminal-user/{id}")]
        public IActionResult EditTerminal(string id, [FromBody] string fullName)
        {
            var terminal = _db.TerminalUsers.FirstOrDefault(x => x.Terminal_ID == id);
            if (terminal == null) return NotFound();

            terminal.FullName = fullName;
            terminal.Update_Date = DateTime.Now;

            _db.SaveChanges();
            return Ok("Terminal updated");
        }

        // ==========================================
        // DELETE TERMINAL USER
        // ==========================================
        [HttpDelete("terminal-user/{id}")]
        public IActionResult DeleteTerminal(string id)
        {
            var terminal = _db.TerminalUsers.FirstOrDefault(x => x.Terminal_ID == id);
            if (terminal == null) return NotFound();

            _db.TerminalUsers.Remove(terminal);
            _db.SaveChanges();

            return Ok("Terminal deleted");
        }
        [HttpGet("report-range")]
        public IActionResult ReportRange(DateTime from, DateTime to)
        {
            var data = _db.KitTransactions
                .Where(x => x.Date >= from.Date && x.Date <= to.Date)
                .Select(x => new
                {
                    Beneficiary_ID = x.Beneficiary_ID,
                    Terminal_ID = x.Terminal_ID,
                    Month = x.Month,
                    Amount = x.Amount == null ? 0 : x.Amount,
                    Status = x.Status
                })
                .ToList();

            return Ok(data);
        }



    }
}
