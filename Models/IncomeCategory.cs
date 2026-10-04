namespace HouseholdExpenseTrackerAPI.Models;

public class IncomeCategory
{
    public int IncomeCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public ICollection<IncomeDetail> IncomeDetails { get; set; } = new List<IncomeDetail>();
}
