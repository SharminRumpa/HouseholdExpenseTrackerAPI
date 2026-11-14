using HouseholdExpenseTrackerAPI.DTOs;
using HouseholdExpenseTrackerAPI.DTOs.AccountTransaction;
using HouseholdExpenseTrackerAPI.Models;
using HouseholdExpenseTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/account-transactions")]
[Authorize]
public class AccountTransactionsController : ControllerBase
{
    private readonly IAccountTransactionService _service;

    public AccountTransactionsController(
        IAccountTransactionService service)
    {
        _service = service;
    }

    private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : throw new InvalidOperationException("Current user ID is not available.");
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetAll(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var isAdmin = User.IsInRole("Admin");

        var result = await _service.GetAllAsync(
            CurrentUserId,
            isAdmin,
            from,
            to);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Member")]
    public async Task<ActionResult<ResponseDto>> Create(
        [FromBody] AccountTransactionCreateDto dto)
    {
        var result = await _service.CreateAsync(
            dto,
            CurrentUserId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
        var result = await _service.DeleteAsync(
            id,
            CurrentUserId,
            User.IsInRole("Admin"));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}