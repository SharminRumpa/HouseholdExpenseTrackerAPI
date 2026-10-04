using HouseholdExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET api/dashboard/summary?year=2026&month=9
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary([FromQuery] int? year, [FromQuery] int? month)
    {
        var now = DateTime.UtcNow;
        var y = year ?? now.Year;
        var m = month ?? now.Month;

        var summary = await _dashboardService.GetSummaryAsync(y, m);
        return Ok(summary);
    }
}
