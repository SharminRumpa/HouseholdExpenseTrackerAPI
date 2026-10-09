using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Expense;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly ApplicationDbContext _db;

        public CategoryService(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Expense Items

        public async Task<List<ExpenseItemResponseDto>> GetAllAsync()
        {
            return await BaseQuery()
                .OrderBy(i => i.ExpenseSubCategory!.ExpenseCategory!.CategoryName)
                .ThenBy(i => i.ExpenseSubCategory!.SubCategoryName)
                .ThenBy(i => i.ItemName)
                .Select(i => ToDto(i))
                .ToListAsync();
        }

        public async Task<ExpenseItemResponseDto?> ExpenseItemGetByIdAsync(int id)
        {
            return await BaseQuery()
                .Where(i => i.ExpenseItemId == id)
                .Select(i => ToDto(i))
                .FirstOrDefaultAsync();
        }

        public async Task<ExpenseItemResponseDto> CreateAsync(ExpenseItemCreateDto dto)
        {
            var itemName = dto.ItemName.Trim();
            await EnsureValidAsync(dto.ExpenseSubCategoryId, itemName, excludeId: null);

            var entity = new ExpenseItem
            {
                ExpenseSubCategoryId = dto.ExpenseSubCategoryId,
                ItemName = itemName,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.ExpenseItems.Add(entity);
            await _db.SaveChangesAsync();

            return (await ExpenseItemGetByIdAsync(entity.ExpenseItemId))!;
        }

        public async Task<ExpenseItemResponseDto?> UpdateAsync(int id, ExpenseItemUpdateDto dto)
        {
            var entity = await _db.ExpenseItems.FirstOrDefaultAsync(i => i.ExpenseItemId == id);
            if (entity is null) return null;

            var itemName = dto.ItemName.Trim();
            await EnsureValidAsync(dto.ExpenseSubCategoryId, itemName, excludeId: id);

            entity.ExpenseSubCategoryId = dto.ExpenseSubCategoryId;
            entity.ItemName = itemName;
            entity.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            entity.IsActive = dto.IsActive;
            entity.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await ExpenseItemGetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.ExpenseItems.FirstOrDefaultAsync(i => i.ExpenseItemId == id);
            if (entity is null) return false;

            // Soft delete — existing ExpenseDetails rows keep pointing at this
            // item, so their history/display names stay intact.
            entity.IsActive = false;
            entity.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        private IQueryable<ExpenseItem> BaseQuery()
        {
            return _db.ExpenseItems
                .AsNoTracking()
                .Include(i => i.ExpenseSubCategory)
                    .ThenInclude(s => s!.ExpenseCategory);
        }

        private async Task EnsureValidAsync(int subCategoryId, string itemName, int? excludeId)
        {
            var subCategoryExists = await _db.ExpenseSubCategories
                .AnyAsync(s => s.ExpenseSubCategoryId == subCategoryId);

            if (!subCategoryExists)
            {
                throw new ArgumentException("The selected sub category does not exist.");
            }

            var duplicate = await _db.ExpenseItems.AnyAsync(i =>
                i.ExpenseSubCategoryId == subCategoryId &&
                i.ItemName == itemName &&
                (excludeId == null || i.ExpenseItemId != excludeId));

            if (duplicate)
            {
                throw new InvalidOperationException("An item with this name already exists in the selected sub category.");
            }
        }

        private static ExpenseItemResponseDto ToDto(ExpenseItem i) => new()
        {
            ExpenseItemId = i.ExpenseItemId,
            ExpenseCategoryId = i.ExpenseSubCategory!.ExpenseCategoryId,
            ExpenseCategoryName = i.ExpenseSubCategory.ExpenseCategory!.CategoryName,
            ExpenseSubCategoryId = i.ExpenseSubCategoryId,
            ExpenseSubCategoryName = i.ExpenseSubCategory.SubCategoryName,
            ItemName = i.ItemName,
            Description = i.Description,
            IsActive = i.IsActive,
            CreatedAt = i.CreatedAt,
            LastUpdatedAt = i.LastUpdatedAt
        };



        #endregion
    }
}
