namespace HouseholdExpenseTrackerAPI.Services;

public interface IPermissionService
{
    Task<List<string>> GetPermissionCodesForRoleAsync(int roleId);
    Task<bool> RoleHasPermissionAsync(int roleId, string permissionCode);
}
