namespace HouseholdExpenseTrackerAPI.Models;

public class ExpenseSubCategory
{
    public int ExpenseSubCategoryId { get; set; }

    public int ExpenseCategoryId { get; set; }
    public ExpenseCategory? ExpenseCategory { get; set; }

    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public ICollection<ExpenseDetail> ExpenseDetails { get; set; } = new List<ExpenseDetail>();
}
