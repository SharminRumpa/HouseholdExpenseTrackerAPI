namespace HouseholdExpenseTrackerAPI.Models;

public class ExpenseDetailsLog
{
    public long ExpenseLogId { get; set; }
    public int ExpenseId { get; set; }

    public int? ExpenseCategoryId { get; set; }
    public int? ExpenseSubCategoryId { get; set; }
    public int? ExpenseItemId { get; set; }

    public DateOnly? ExpenseDate { get; set; }
    public decimal? Amount { get; set; }

    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }

    public decimal? NetWeight { get; set; }
    public string? WeightUnit { get; set; }

    public string? PaymentMethod { get; set; }
    public string? ExpenseBy { get; set; }
    public string? ExpenseFor { get; set; }
    public string? Description { get; set; }

    public string ActionType { get; set; } = string.Empty;
    public int? ActionByUserId { get; set; }
    public DateTime ActionDate { get; set; }
}

