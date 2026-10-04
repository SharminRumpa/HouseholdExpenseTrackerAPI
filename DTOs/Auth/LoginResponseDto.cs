namespace HouseholdExpenseTrackerAPI.DTOs.Auth;

// One Permission row = one navigable menu entry (it already carries
// Title/Icon/Route). "Permissions" is kept as an array (just the one
// code) so the frontend route guard / UI can check it generically,
// and so this can extend to multiple codes per item later without a
// breaking shape change.
public class MenuItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Route { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
