using System.Security.Claims;
using HouseholdExpenseTrackerAPI.DTOs.Expense;
using HouseholdExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : null;

    [HttpGet]
    public async Task<ActionResult<List<ExpenseResponseDto>>> GetAll(
        [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? categoryId)
    {
        return Ok(await _expenseService.GetAllAsync(from, to, categoryId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseResponseDto>> GetById(int id)
    {
        var result = await _expenseService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    //[Authorize(Roles = "Admin,Member")]
    public async Task<ActionResult<ExpenseResponseDto>> Create(ExpenseCreateDto dto)
    {
        var result = await _expenseService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.ExpenseId }, result);
    }

    [HttpPut("{id:int}")]
    //[Authorize(Roles = "Admin,Member")]
    public async Task<ActionResult<ExpenseResponseDto>> Update(int id, ExpenseUpdateDto dto)
    {
        var result = await _expenseService.UpdateAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = "Admin,Member")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _expenseService.DeleteAsync(id, CurrentUserId);
        return deleted ? NoContent() : NotFound();
    }
}
