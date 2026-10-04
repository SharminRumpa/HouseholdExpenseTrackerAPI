using HouseholdExpenseTrackerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public RoleController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _db.Roles
            .OrderBy(r => r.RoleId)
            .Select(r => new { r.RoleId, r.RoleName })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id:int}/permissions")]
    public async Task<IActionResult> GetPermissions(int id)
    {
        var permissions = await _db.RolePermissions
            .Where(rp => rp.RoleId == id)
            .Select(rp => new
            {
                rp.MenuDetail!.MenuCode,
                rp.MenuDetail.MenuName,
                rp.MenuDetail.Route,
                rp.MenuDetail.Icon
            })
            .ToListAsync();

        return Ok(permissions);
    }
}
