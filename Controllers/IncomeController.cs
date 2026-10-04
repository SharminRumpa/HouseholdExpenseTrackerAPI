using System.Security.Claims;
using HouseholdExpenseTrackerAPI.DTOs.Income;
using HouseholdExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomeController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : null;

    [HttpGet]
    public async Task<ActionResult<List<IncomeResponseDto>>> GetAll(
        [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? categoryId)
    {
        return Ok(await _incomeService.GetAllAsync(from, to, categoryId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncomeResponseDto>> GetById(int id)
    {
        var result = await _incomeService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    //[Authorize(Roles = "Admin,Member")]
    public async Task<ActionResult<IncomeResponseDto>> Create(IncomeCreateDto dto)
    {
        var result = await _incomeService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.IncomeId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Member")]
    public async Task<ActionResult<IncomeResponseDto>> Update(int id, IncomeUpdateDto dto)
    {
        var result = await _incomeService.UpdateAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Member")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _incomeService.DeleteAsync(id, CurrentUserId);
        return deleted ? NoContent() : NotFound();
    }
}
