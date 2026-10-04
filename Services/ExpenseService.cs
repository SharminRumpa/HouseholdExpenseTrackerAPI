using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Expense;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services;

public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _db;

    public ExpenseService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ExpenseResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId)
    {
        var query = _db.ExpenseDetails
            .Include(e => e.ExpenseCategory)
            .Include(e => e.ExpenseSubCategory)
            .AsQueryable();

        if (from.HasValue) query = query.Where(e => e.ExpenseDate >= from.Value);
        if (to.HasValue) query = query.Where(e => e.ExpenseDate <= to.Value);
        if (categoryId.HasValue) query = query.Where(e => e.ExpenseCategoryId == categoryId.Value);

        return await query
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => ToDto(e))
            .ToListAsync();
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.ExpenseDetails
            .Include(e => e.ExpenseCategory)
            .Include(e => e.ExpenseSubCategory)
            .FirstOrDefaultAsync(e => e.ExpenseId == id);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto, int? currentUserId)
    {
        var entity = new ExpenseDetail
        {
            ExpenseCategoryId = dto.ExpenseCategoryId,
            ExpenseSubCategoryId = dto.ExpenseSubCategoryId,
            ExpenseItemId = dto.ExpenseItemId,
            ExpenseDate = dto.ExpenseDate,
            Amount = dto.Amount,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            NetWeight = dto.NetWeight,
            WeightUnit = dto.WeightUnit,
            PaymentMethod = dto.PaymentMethod,
            ExpenseBy = dto.ExpenseBy,
            ExpenseFor = dto.ExpenseFor,
            Description = dto.Description,
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            LastUpdatedByUserId = currentUserId
        };

        _db.ExpenseDetails.Add(entity);
        await _db.SaveChangesAsync();
        await LogAsync(entity, "INSERT", currentUserId);

        return await GetByIdAsync(entity.ExpenseId) ?? ToDto(entity);
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, ExpenseUpdateDto dto, int? currentUserId)
    {
        var entity = await _db.ExpenseDetails.FirstOrDefaultAsync(e => e.ExpenseId == id);
        if (entity is null) return null;

        entity.ExpenseCategoryId = dto.ExpenseCategoryId;
        entity.ExpenseSubCategoryId = dto.ExpenseSubCategoryId;
        entity.ExpenseDate = dto.ExpenseDate;
        entity.Amount = dto.Amount;
        entity.Quantity = dto.Quantity;
        entity.Unit = dto.Unit;
        entity.NetWeight = dto.NetWeight;
        entity.WeightUnit = dto.WeightUnit;
        entity.PaymentMethod = dto.PaymentMethod;
        entity.ExpenseBy = dto.ExpenseBy;
        entity.Description = dto.Description;
        entity.LastUpdatedByUserId = currentUserId;
        entity.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await LogAsync(entity, "UPDATE", currentUserId);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id, int? currentUserId)
    {
        var entity = await _db.ExpenseDetails.FirstOrDefaultAsync(e => e.ExpenseId == id);
        if (entity is null) return false;

        await LogAsync(entity, "DELETE", currentUserId);

        _db.ExpenseDetails.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task LogAsync(ExpenseDetail entity, string actionType, int? actionByUserId)
    {
        _db.Set<ExpenseDetailsLog>().Add(new ExpenseDetailsLog
        {
            ExpenseId = entity.ExpenseId,
            ExpenseCategoryId = entity.ExpenseCategoryId,
            ExpenseSubCategoryId = entity.ExpenseSubCategoryId,
            ExpenseItemId = entity.ExpenseItemId,
            ExpenseDate = entity.ExpenseDate,
            Amount = entity.Amount,
            Quantity = entity.Quantity,
            Unit = entity.Unit,
            NetWeight = entity.NetWeight,
            WeightUnit = entity.WeightUnit,
            PaymentMethod = entity.PaymentMethod,
            ExpenseBy = entity.ExpenseBy,
            ExpenseFor = entity.ExpenseFor,
            Description = entity.Description,
            ActionType = actionType,
            ActionByUserId = actionByUserId,
            ActionDate = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    private static ExpenseResponseDto ToDto(ExpenseDetail e) => new()
    {
        ExpenseId = e.ExpenseId,
        ExpenseCategoryId = e.ExpenseCategoryId,
        ExpenseCategoryName = e.ExpenseCategory?.CategoryName ?? string.Empty,
        ExpenseSubCategoryId = e.ExpenseSubCategoryId,
        ExpenseSubCategoryName = e.ExpenseSubCategory?.SubCategoryName,
        ExpenseDate = e.ExpenseDate,
        Amount = e.Amount,
        Quantity = e.Quantity,
        Unit = e.Unit,
        NetWeight = e.NetWeight,
        WeightUnit = e.WeightUnit,
        PaymentMethod = e.PaymentMethod,
        ExpenseBy = e.ExpenseBy,
        Description = e.Description,
        CreatedAt = e.CreatedAt,
        LastUpdatedAt = e.LastUpdatedAt
    };
}
