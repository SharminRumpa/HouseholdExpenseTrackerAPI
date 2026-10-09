namespace HouseholdExpenseTrackerAPI.DTOs.Expense;

public class ExpenseUpdateDto
{
    public int ExpenseCategoryId { get; set; }
    public int? ExpenseSubCategoryId { get; set; }
    public int? ExpenseItemId { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? NetWeight { get; set; }
    public string? WeightUnit { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ExpenseBy { get; set; }
    public string? ExpenseFor { get; set; }
    public string? Description { get; set; }
}

