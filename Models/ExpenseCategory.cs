namespace HouseholdExpenseTrackerAPI.Models;

public class ExpenseCategory
{
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public ICollection<ExpenseSubCategory> SubCategories { get; set; } = new List<ExpenseSubCategory>();
    public ICollection<ExpenseDetail> ExpenseDetails { get; set; } = new List<ExpenseDetail>();
}
