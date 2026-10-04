namespace HouseholdExpenseTrackerAPI.Models
{
    public class ExpenseItem
    {
        public int ExpenseItemId { get; set; }
        public int ExpenseSubCategoryId { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public ExpenseSubCategory? ExpenseSubCategory { get; set; }
    }
}
