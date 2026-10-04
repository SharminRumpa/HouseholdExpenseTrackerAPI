namespace HouseholdExpenseTrackerAPI.Dtos
{
    // Generic shape for a simple Id/Name dropdown (Income Categories, Income Sources,
    // Expense Categories, Roles).
    public class DropdownItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Expense sub-categories carry their parent category id so the UI can filter
    // the sub-category dropdown when the user picks an Expense Category.
    public class ExpenseSubCategoryDropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ExpenseCategoryId { get; set; }
    }
}
