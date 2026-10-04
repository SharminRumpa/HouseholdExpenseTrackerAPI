using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Category;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ---------------- Income Categories ----------------

    [HttpGet("income")]
    public async Task<ActionResult<List<CategoryDto>>> GetIncomeCategories()
    {
        var result = await _db.IncomeCategories
            .Select(c => new CategoryDto
            {
                CategoryId = c.IncomeCategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                IsActive = c.IsActive
            })
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        return Ok(result);
    }

    [HttpPost("income")]
    public async Task<ActionResult<CategoryDto>> CreateIncomeCategory(CategoryCreateDto dto)
    {
        var category = new IncomeCategory
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.IncomeCategories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(new CategoryDto
        {
            CategoryId = category.IncomeCategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        });
    }

    [HttpPut("income/{id:int}")]
    public async Task<ActionResult<CategoryDto>> UpdateIncomeCategory(int id, CategoryUpdateDto dto)
    {
        var category = await _db.IncomeCategories.FirstOrDefaultAsync(c => c.IncomeCategoryId == id);
        if (category is null) return NotFound();

        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new CategoryDto
        {
            CategoryId = category.IncomeCategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        });
    }

    [HttpDelete("income/{id:int}")]
    public async Task<IActionResult> DeleteIncomeCategory(int id)
    {
        var category = await _db.IncomeCategories.FirstOrDefaultAsync(c => c.IncomeCategoryId == id);
        if (category is null) return NotFound();

        _db.IncomeCategories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ---------------- Expense Categories ----------------

    [HttpGet("expense")]
    public async Task<ActionResult<List<CategoryDto>>> GetExpenseCategories()
    {
        var result = await _db.ExpenseCategories
            .Select(c => new CategoryDto
            {
                CategoryId = c.ExpenseCategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                IsActive = c.IsActive
            })
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        return Ok(result);
    }

    [HttpPut("expense/{id:int}")]
    public async Task<ActionResult<CategoryDto>> UpdateExpenseCategory(int id, CategoryUpdateDto dto)
    {
        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(c => c.ExpenseCategoryId == id);
        if (category is null) return NotFound();

        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new CategoryDto
        {
            CategoryId = category.ExpenseCategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        });
    }

    [HttpDelete("expense/{id:int}")]
    public async Task<IActionResult> DeleteExpenseCategory(int id)
    {
        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(c => c.ExpenseCategoryId == id);
        if (category is null) return NotFound();

        _db.ExpenseCategories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("expense/{categoryId:int}/subcategories")]
    public async Task<ActionResult<List<SubCategoryDto>>> GetExpenseSubCategories(int categoryId)
    {
        var result = await _db.ExpenseSubCategories
            .Where(sc => sc.ExpenseCategoryId == categoryId && sc.IsActive)
            .Select(sc => new SubCategoryDto
            {
                SubCategoryId = sc.ExpenseSubCategoryId,
                CategoryId = sc.ExpenseCategoryId,
                SubCategoryName = sc.SubCategoryName,
                Description = sc.Description,
                IsActive = sc.IsActive
            })
            .OrderBy(sc => sc.SubCategoryName)
            .ToListAsync();

        return Ok(result);
    }

    /// <summary>
    ///  // ---------------- Expense Sub Categories  ----------------

    [HttpGet("expense-subcategories")]
    public async Task<ActionResult<List<SubCategoryDto>>> GetExpenseSubCategories()
    {
        var result = await _db.ExpenseSubCategories
            .Select(sc => new SubCategoryDto
            {
                SubCategoryId = sc.ExpenseSubCategoryId,
                CategoryId = sc.ExpenseCategoryId,
                CategoryName = sc.ExpenseCategory.CategoryName ?? "-",
                SubCategoryName = sc.SubCategoryName,
                Description = sc.Description,
                IsActive = sc.IsActive
            })
            .OrderBy(sc => sc.SubCategoryName)
            .ToListAsync();

        return Ok(result);
    }
    /// </summary>
    /// <returns></returns>


    // ---------------- Income Sources ----------------

    [HttpGet("income-sources")]
    public async Task<ActionResult<List<IncomeSourceDto>>> GetIncomeSources()
    {
        var result = await _db.IncomeSources
            .Select(s => new IncomeSourceDto
            {
                IncomeSourceId = s.IncomeSourceId,
                SourceName = s.SourceName,
                Description = s.Description,
                IsActive = s.IsActive
            })
            .OrderBy(s => s.SourceName)
            .ToListAsync();

        return Ok(result);
    }

    [HttpPost("income-sources")]
    public async Task<ActionResult<IncomeSourceDto>> CreateIncomeSource(IncomeSourceCreateDto dto)
    {
        var source = new IncomeSource
        {
            SourceName = dto.SourceName,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.IncomeSources.Add(source);
        await _db.SaveChangesAsync();

        return Ok(new IncomeSourceDto
        {
            IncomeSourceId = source.IncomeSourceId,
            SourceName = source.SourceName,
            Description = source.Description,
            IsActive = source.IsActive
        });
    }

    [HttpPut("income-sources/{id:int}")]
    public async Task<ActionResult<IncomeSourceDto>> UpdateIncomeSource(int id, IncomeSourceUpdateDto dto)
    {
        var source = await _db.IncomeSources.FirstOrDefaultAsync(s => s.IncomeSourceId == id);
        if (source is null) return NotFound();

        source.SourceName = dto.SourceName;
        source.Description = dto.Description;
        source.IsActive = dto.IsActive;
        source.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new IncomeSourceDto
        {
            IncomeSourceId = source.IncomeSourceId,
            SourceName = source.SourceName,
            Description = source.Description,
            IsActive = source.IsActive
        });
    }

    [HttpDelete("income-sources/{id:int}")]
    public async Task<IActionResult> DeleteIncomeSource(int id)
    {
        var source = await _db.IncomeSources.FirstOrDefaultAsync(s => s.IncomeSourceId == id);
        if (source is null) return NotFound();

        _db.IncomeSources.Remove(source);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}