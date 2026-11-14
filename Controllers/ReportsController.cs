using HouseholdExpenseTrackerAPI.DTOs.Report;
using HouseholdExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET api/reports/expenses?from=2026-10-01&to=2026-10-13&categoryId=1&subCategoryId=4&expenseBy=Sharmin&expenseFor=Piash
        [HttpGet("expenses")]
        public async Task<ActionResult<ExpenseReportDto>> GetExpenseReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? categoryId,
            [FromQuery] int? subCategoryId,
            [FromQuery] string? expenseBy,
            [FromQuery] string? expenseFor)
        {
            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
            {
                return BadRequest(new { message = "From date cannot be after To date." });
            }

            return Ok(await _reportService.GetExpenseReportAsync(from, to, categoryId, subCategoryId, expenseBy, expenseFor));
        }

        // GET api/reports/expenses/export?...same filters...   -> .xlsx download
        [HttpGet("expenses/export")]
        public async Task<IActionResult> ExportExpenseReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? categoryId,
            [FromQuery] int? subCategoryId,
            [FromQuery] string? expenseBy,
            [FromQuery] string? expenseFor)
        {
            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
            {
                return BadRequest(new { message = "From date cannot be after To date." });
            }

            var bytes = await _reportService.ExportExpenseReportAsync(
                from, to, categoryId, subCategoryId, expenseBy, expenseFor);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"expense-report-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx");
        }

        
    }
}
