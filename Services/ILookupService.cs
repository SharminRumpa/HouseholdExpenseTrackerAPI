using HouseholdExpenseTrackerAPI.Dtos;

namespace HouseholdExpenseTrackerAPI.Services
{
    public interface ILookupService
    {
        Task<List<DropdownItemDto>> GetIncomeCategoriesAsync();
        Task<List<DropdownItemDto>> GetIncomeSourcesAsync();
        Task<List<DropdownItemDto>> GetExpenseCategoriesAsync();
        Task<List<ExpenseSubCategoryDropdownDto>> GetExpenseSubCategoriesAsync(int? expenseCategoryId);
        Task<IReadOnlyList<DropdownItemDto>> GetExpenseItemsAsync(int expenseSubCategoryId);
        Task<List<DropdownItemDto>> GetRolesAsync();

        // Distinct, non-empty ExpenseBy values for the "person" dropdown.
        Task<List<string>> GetExpensePersonsAsync();

        // Distinct, non-empty ExpenseFor values for the "expense for" dropdown.
        Task<List<string>> GetExpenseForPersonsAsync();
    }
}
