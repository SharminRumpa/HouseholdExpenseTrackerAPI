using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Category;
using HouseholdExpenseTrackerAPI.DTOs.Expense;
using HouseholdExpenseTrackerAPI.Models;
using HouseholdExpenseTrackerAPI.Services.Interfaces;
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


        #region Expense Sub Category

        public async Task<List<SubCategoryDto>> SubCategoryGetAllAsync()
        {
            return await SubCategoryBaseQuery()
                .OrderBy(s => s.ExpenseCategory.CategoryName)
                .ThenBy(s => s.SubCategoryName)
                .Select(s => ToDto(s))
                .ToListAsync();
        }

        public async Task<SubCategoryDto?> SubCategoryGetByIdAsync(int id)
        {
            return await SubCategoryBaseQuery()
                .Where(s => s.ExpenseSubCategoryId == id)
                .Select(s => ToDto(s))
                .FirstOrDefaultAsync();
        }

        public async Task<SubCategoryDto> SubCategoryCreateAsync(ExpenseSubCategorySaveDto dto)
        {
            var name = dto.SubCategoryName.Trim();
            await EnsureValidAsync(dto.ExpenseCategoryId, name, excludeId: null);

            var entity = new ExpenseSubCategory
            {
                ExpenseCategoryId = dto.ExpenseCategoryId,
                SubCategoryName = name,
                Description = CleanText(dto.Description),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.ExpenseSubCategories.Add(entity);
            await _db.SaveChangesAsync();

            return (await SubCategoryGetByIdAsync(entity.ExpenseCategoryId))!;
        }

        public async Task<SubCategoryDto?> SubCategoryUpdateAsync(int id, ExpenseSubCategorySaveDto dto)
        {
            var entity = await _db.ExpenseSubCategories.FirstOrDefaultAsync(s => s.ExpenseSubCategoryId == id);
            if (entity is null) return null;

            var name = dto.SubCategoryName.Trim();
            await SubCategoryEnsureValidAsync(dto.ExpenseCategoryId, name, excludeId: id);

            entity.ExpenseCategoryId = dto.ExpenseCategoryId;
            entity.SubCategoryName = name;
            entity.Description = CleanText(dto.Description);
            entity.IsActive = dto.IsActive;
            entity.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await SubCategoryGetByIdAsync(id);
        }

        public async Task<SubCategoryDto?> SetActiveAsync(int id, bool isActive)
        {
            var entity = await _db.ExpenseSubCategories.FirstOrDefaultAsync(s => s.ExpenseSubCategoryId == id);
            if (entity is null) return null;

            entity.IsActive = isActive;
            entity.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await SubCategoryGetByIdAsync(id);
        }

        public async Task<bool> SubCategoryDeleteAsync(int id)
        {
            var entity = await _db.ExpenseSubCategories.FirstOrDefaultAsync(s => s.ExpenseSubCategoryId == id);
            if (entity is null) return false;

            _db.ExpenseSubCategories.Remove(entity);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Foreign keys from ExpenseItems / ExpenseDetails block the delete.
                throw new InvalidOperationException(
                    "This sub-category is already in use and cannot be deleted. Deactivate it instead.");
            }

            return true;
        }

        private IQueryable<ExpenseSubCategory> SubCategoryBaseQuery()
        {
            return _db.ExpenseSubCategories
                .AsNoTracking()
                .Include(s => s.ExpenseCategory);
        }

        private async Task SubCategoryEnsureValidAsync(int categoryId, string name, int? excludeId)
        {
            var categoryExists = await _db.ExpenseCategories
                .AnyAsync(c => c.ExpenseCategoryId == categoryId);

            if (!categoryExists)
            {
                throw new ArgumentException("The selected category does not exist.");
            }

            var duplicate = await _db.ExpenseSubCategories.AnyAsync(s =>
                s.ExpenseCategoryId == categoryId &&
                s.SubCategoryName == name &&
                (excludeId == null || s.ExpenseSubCategoryId != excludeId));

            if (duplicate)
            {
                throw new InvalidOperationException("A sub-category with this name already exists in the selected category.");
            }
        }

        private static string? CleanText(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static SubCategoryDto ToDto(ExpenseSubCategory s) => new()
        {
            SubCategoryId = s.ExpenseSubCategoryId,
            CategoryId = s.ExpenseCategoryId,
            CategoryName = s.ExpenseCategory!.CategoryName,
            SubCategoryName = s.SubCategoryName,
            Description = s.Description,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            LastUpdatedAt = s.LastUpdatedAt
        };

        #endregion
    }
}
