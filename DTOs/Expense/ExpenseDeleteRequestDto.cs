namespace HouseholdExpenseTrackerAPI.DTOs.Expense
{
    public class ExpenseDeleteRequestDto
    {
        public int ExpenseId { get; set; }
        public string? Reason { get; set; }
    }

    public class ExpenseDeleteReviewDto
    {
        public string? ReviewComment { get; set; }
    }
}
