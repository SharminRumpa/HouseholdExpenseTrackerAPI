namespace HouseholdExpenseTrackerAPI.Models
{
    public class ExpenseDeleteRequest
    {
        public int ExpenseDeleteRequestId { get; set; }

        public int? ExpenseId { get; set; }

        public int RequestedByUserId { get; set; }
        public string? Reason { get; set; }

        public string Status { get; set; } = "Pending";

        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewComment { get; set; }

        public DateTime CreatedAt { get; set; }

        public ExpenseDetail? Expense { get; set; }

        public User? RequestedByUser { get; set; }
        public User? ReviewedByUser { get; set; }
    }
}
