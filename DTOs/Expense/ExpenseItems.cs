namespace HouseholdExpenseTrackerAPI.DTOs.Expense
{
    public class ExpenseItems
    {
    }

    public class ExpenseItemCreateDto
    {
        // Only the sub-category is stored on the entity — the master
        // category is derived through it (see ToDto in the service).
        public int ExpenseSubCategoryId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ExpenseItemUpdateDto
    {
        public int ExpenseSubCategoryId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        // Editable here too — this is how a soft-deleted item gets
        // reactivated, since the Delete button only ever turns this off.
        public bool IsActive { get; set; }
    }

    public class ExpenseItemResponseDto
    {
        public int ExpenseItemId { get; set; }
        public int ExpenseCategoryId { get; set; }
        public string ExpenseCategoryName { get; set; } = string.Empty;
        public int ExpenseSubCategoryId { get; set; }
        public string ExpenseSubCategoryName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
