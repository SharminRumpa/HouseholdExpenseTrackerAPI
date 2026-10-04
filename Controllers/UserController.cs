using Azure.Core;
using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Income;
using HouseholdExpenseTrackerAPI.DTOs.User;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : null;

    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        var users = await _db.Users
            .Include(u => u.Role)
            .Select(u => ToDto(u))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        return user is null ? NotFound() : Ok(ToDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(UserCreateDto dto)
    {
        var ExistingUser = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email || u.MobileNumber == dto.MobileNumber);
        if (ExistingUser is not null) return BadRequest("User with this email or mobile number already exists.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            MobileNumber = dto.MobileNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = dto.RoleId,
            IsActive = true,
            CreatedBy = CurrentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = CurrentUserId,
            LastUpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        await _db.Entry(user).Reference(u => u.Role).LoadAsync();

        return Ok(ToDto(user));
    } 

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
    {
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        if (user is null) return NotFound();

        var ExistingUser = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber && u.UserId != id);
        if (ExistingUser is not null) return BadRequest("User with this email or mobile number already exists.");

        user.Name = dto.Name;
        user.MobileNumber = dto.MobileNumber;
        user.RoleId = dto.RoleId;
        user.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(user).Reference(u => u.Role).LoadAsync();

        return Ok(ToDto(user));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (user is null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static UserResponseDto ToDto(User u) => new()
    {
        UserId = u.UserId,
        Name = u.Name,
        Email = u.Email,
        MobileNumber = u.MobileNumber,
        RoleId = u.RoleId,
        RoleName = u.Role?.RoleName ?? string.Empty,
        CreatedAt = u.CreatedAt,
        LastUpdatedAt = u.LastUpdatedAt
    };
}
