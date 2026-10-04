using HouseholdExpenseTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _db;

    public PermissionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<string>> GetPermissionCodesForRoleAsync(int roleId)
    {
        return await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.MenuDetail!.MenuCode)
            .ToListAsync();
    }

    public async Task<bool> RoleHasPermissionAsync(int roleId, string permissionCode)
    {
        return await _db.RolePermissions
            .Include(rp => rp.MenuDetail)
            .AnyAsync(rp => rp.RoleId == roleId && rp.MenuDetail!.MenuCode == permissionCode);
    }
}
