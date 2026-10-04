namespace HouseholdExpenseTrackerAPI.DTOs.User;

public class UserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public int RoleId { get; set; }
}
