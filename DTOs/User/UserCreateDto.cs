namespace HouseholdExpenseTrackerAPI.DTOs.User;

public class UserCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
}
