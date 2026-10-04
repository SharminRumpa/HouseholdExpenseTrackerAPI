namespace HouseholdExpenseTrackerAPI.Models;

public class RolePermission
{
    public int RolePermissionId { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public int MenuDetailsId { get; set; }
    public MenuDetail? MenuDetail { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
