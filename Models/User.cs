namespace HouseholdExpenseTrackerAPI.Models;

public class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }
    public User? UpdatedByUser { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public Role? Role { get; set; }
}

