namespace HouseholdExpenseTrackerAPI.DTOs.Expense;

public class ExpenseResponseDto
{
    public int ExpenseId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public string ExpenseCategoryName { get; set; } = string.Empty;
    public int? ExpenseSubCategoryId { get; set; }
    public string? ExpenseSubCategoryName { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? NetWeight { get; set; }
    public string? WeightUnit { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ExpenseBy { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
