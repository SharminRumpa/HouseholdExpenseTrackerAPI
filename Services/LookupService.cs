using HouseholdExpenseTrackerAPI.Data; // adjust to your actual DbContext namespace
using Microsoft.EntityFrameworkCore;
using HouseholdExpenseTrackerAPI.Dtos;

namespace HouseholdExpenseTrackerAPI.Services
{
    public class LookupService : ILookupService         
    {
        private readonly ApplicationDbContext _db;

        public LookupService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<DropdownItemDto>> GetIncomeCategoriesAsync()
        {
            return await _db.IncomeCategories
                .AsNoTracking()         
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .Select(c => new DropdownItemDto { Id = c.IncomeCategoryId, Name = c.CategoryName })
                .ToListAsync();
        }

        public async Task<List<DropdownItemDto>> GetIncomeSourcesAsync()
        {
            return await _db.IncomeSources
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SourceName)
                .Select(s => new DropdownItemDto { Id = s.IncomeSourceId, Name = s.SourceName })
                .ToListAsync();
        }

        public async Task<List<DropdownItemDto>> GetExpenseCategoriesAsync()
        {
            return await _db.ExpenseCategories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .Select(c => new DropdownItemDto { Id = c.ExpenseCategoryId, Name = c.CategoryName })
                .ToListAsync();
        }

        public async Task<List<ExpenseSubCategoryDropdownDto>> GetExpenseSubCategoriesAsync(int? expenseCategoryId)
        {
            var query = _db.ExpenseSubCategories
                .AsNoTracking()
                .Where(sc => sc.IsActive)
                .AsQueryable();

            if (expenseCategoryId.HasValue)
            {
                query = query.Where(sc => sc.ExpenseCategoryId == expenseCategoryId.Value);
            }

            return await query
                .OrderBy(sc => sc.SubCategoryName)
                .Select(sc => new ExpenseSubCategoryDropdownDto
                {
                    Id = sc.ExpenseSubCategoryId,
                    Name = sc.SubCategoryName,
                    ExpenseCategoryId = sc.ExpenseCategoryId
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<DropdownItemDto>> GetExpenseItemsAsync(int expenseSubCategoryId)
        {
            return await _db.ExpenseItems
                .AsNoTracking()
                .Where(x => x.IsActive && x.ExpenseSubCategoryId == expenseSubCategoryId)
                .OrderBy(x => x.ItemName)
                .Select(x => new DropdownItemDto
                {
                    Id = x.ExpenseItemId,
                    Name = x.ItemName
                })
                .ToListAsync();
        }

        public async Task<List<DropdownItemDto>> GetRolesAsync()
        {
            // Roles has no IsActive column, so all roles are returned.
            return await _db.Roles
                .AsNoTracking()
                .OrderBy(r => r.RoleName)
                .Select(r => new DropdownItemDto { Id = r.RoleId, Name = r.RoleName })
                .ToListAsync();
        }
    }
}
