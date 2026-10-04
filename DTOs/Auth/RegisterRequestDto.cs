namespace HouseholdExpenseTrackerAPI.DTOs.Auth;

public class RegisterRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string Password { get; set; } = string.Empty;

    // Defaults to "Member" in AuthService if not supplied.
    public string? RoleName { get; set; }
}
