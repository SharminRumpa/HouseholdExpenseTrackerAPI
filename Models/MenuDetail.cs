namespace HouseholdExpenseTrackerAPI.Models;

public class MenuDetail
{
    public int MenuDetailsId { get; set; }

    public string MenuCode { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }

    public bool IsActive { get; set; }
    public int OrderBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
