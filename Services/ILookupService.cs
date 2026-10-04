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
    }
}
