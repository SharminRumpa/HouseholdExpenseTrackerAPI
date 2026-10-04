namespace HouseholdExpenseTrackerAPI.DTOs.User;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
