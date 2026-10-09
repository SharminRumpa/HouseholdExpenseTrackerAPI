using HouseholdExpenseTrackerAPI.DTOs.Expense;

namespace HouseholdExpenseTrackerAPI.Services
{
    public interface ICategoryService
    {
        #region Expense Items

        Task<List<ExpenseItemResponseDto>> GetAllAsync();
        Task<ExpenseItemResponseDto?> ExpenseItemGetByIdAsync(int id);

        // Throws ArgumentException if the sub-category doesn't exist,
        // InvalidOperationException if the name already exists under it.
        Task<ExpenseItemResponseDto> CreateAsync(ExpenseItemCreateDto dto);
        Task<ExpenseItemResponseDto?> UpdateAsync(int id, ExpenseItemUpdateDto dto);

        // Soft delete: sets IsActive = false. Returns false if not found.
        Task<bool> DeleteAsync(int id);

        #endregion
    }
}
