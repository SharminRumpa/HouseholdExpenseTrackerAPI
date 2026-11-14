using HouseholdExpenseTrackerAPI.DTOs.Category;
using HouseholdExpenseTrackerAPI.DTOs.Expense;

namespace HouseholdExpenseTrackerAPI.Services.Interfaces
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


        #region Expense Sub Category

        Task<List<SubCategoryDto>> SubCategoryGetAllAsync();
        Task<SubCategoryDto?> SubCategoryGetByIdAsync(int id);

        // Throws ArgumentException if the category doesn't exist,
        // InvalidOperationException if the name already exists under it.
        Task<SubCategoryDto> SubCategoryCreateAsync(ExpenseSubCategorySaveDto dto);
        Task<SubCategoryDto?> SubCategoryUpdateAsync(int id, ExpenseSubCategorySaveDto dto);

        // Flips IsActive only. Returns null if not found.
        Task<SubCategoryDto?> SetActiveAsync(int id, bool isActive);

        // Hard delete. Returns false if not found; throws InvalidOperationException
        // if expense items / expenses still reference it.
        Task<bool> SubCategoryDeleteAsync(int id);

        #endregion
    }
}
