using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Income;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services;

public class IncomeService : IIncomeService
{
    private readonly ApplicationDbContext _db;

    public IncomeService(ApplicationDbContext db)
    {
        _db = db;
    }

    //public async Task<List<IncomeResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId)
    //{
    //    var query = _db.IncomeDetails
    //        .Include(i => i.IncomeCategory)
    //        .Include(i => i.IncomeSource)
    //        .AsQueryable();

    //    if (from.HasValue) query = query.Where(i => i.ReceivedDate >= from.Value);
    //    if (to.HasValue) query = query.Where(i => i.ReceivedDate <= to.Value);
    //    if (categoryId.HasValue) query = query.Where(i => i.IncomeCategoryId == categoryId.Value);

    //    return await query
    //        .OrderByDescending(i => i.ReceivedDate)
    //        .Select(i => ToDto(i))
    //        .ToListAsync();
    //}

    // Drop these members into your existing service/repository class alongside ToDto(...).
    // Only the query method and the two small helpers below are new/changed.

    public async Task<List<IncomeResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId)
    {
        var hasNoFilters = !from.HasValue && !to.HasValue && !categoryId.HasValue;

        if (hasNoFilters)
        {
            (from, to) = GetCurrentMonthRange();
        }

        var query = BuildFilteredQuery(from, to, categoryId);

        return await query
            .OrderByDescending(i => i.ReceivedDate)
            .Select(i => ToDto(i))
            .ToListAsync();
    }

    private IQueryable<IncomeDetail> BuildFilteredQuery(DateOnly? from, DateOnly? to, int? categoryId)
    {
        var query = _db.IncomeDetails
            .AsNoTracking()
            .Include(i => i.IncomeCategory)
            .Include(i => i.IncomeSource)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(i => i.ReceivedDate >= from.Value);

        if (to.HasValue)
            query = query.Where(i => i.ReceivedDate <= to.Value);

        if (categoryId.HasValue)
            query = query.Where(i => i.IncomeCategoryId == categoryId.Value);

        return query;
    }

    private static (DateOnly From, DateOnly To) GetCurrentMonthRange()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
        return (firstDayOfMonth, today);
    }

    public async Task<IncomeResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.IncomeDetails
            .Include(i => i.IncomeCategory)
            .Include(i => i.IncomeSource)
            .FirstOrDefaultAsync(i => i.IncomeId == id);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IncomeResponseDto> CreateAsync(IncomeCreateDto dto, int? currentUserId)
    {
        var entity = new IncomeDetail
        {
            IncomeCategoryId = dto.IncomeCategoryId,
            IncomeSourceId = dto.IncomeSourceId,
            ReceivedDate = dto.ReceivedDate,
            ActualDate = dto.ActualDate,
            Amount = dto.Amount,
            Description = dto.Description,
            PaymentMethod = dto.PaymentMethod,
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedByUserId = currentUserId,
            LastUpdatedAt = DateTime.UtcNow,
            Year = dto.ReceivedDate.Year,
            Month = dto.Month
        };

        _db.IncomeDetails.Add(entity);
        await _db.SaveChangesAsync();
        await LogAsync(entity, "INSERT", currentUserId);

        return await GetByIdAsync(entity.IncomeId) ?? ToDto(entity);
    }

    public async Task<IncomeResponseDto?> UpdateAsync(int id, IncomeUpdateDto dto, int? currentUserId)
    {
        var entity = await _db.IncomeDetails.FirstOrDefaultAsync(i => i.IncomeId == id);
        if (entity is null) return null;

        entity.IncomeCategoryId = dto.IncomeCategoryId;
        entity.IncomeSourceId = dto.IncomeSourceId;
        entity.ReceivedDate = dto.ReceivedDate;
        entity.ActualDate = dto.ActualDate;
        entity.Amount = dto.Amount;
        entity.Description = dto.Description;
        entity.PaymentMethod = dto.PaymentMethod;
        entity.LastUpdatedByUserId = currentUserId;
        entity.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await LogAsync(entity, "UPDATE", currentUserId);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id, int? currentUserId)
    {
        var entity = await _db.IncomeDetails.FirstOrDefaultAsync(i => i.IncomeId == id);
        if (entity is null) return false;

        await LogAsync(entity, "DELETE", currentUserId);

        _db.IncomeDetails.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task LogAsync(IncomeDetail entity, string actionType, int? actionByUserId)
    {
        _db.Set<IncomeDetailsLog>().Add(new IncomeDetailsLog
        {
            IncomeId = entity.IncomeId,
            IncomeCategoryId = entity.IncomeCategoryId,
            IncomeSourceId = entity.IncomeSourceId,
            ReceivedDate = entity.ReceivedDate,
            ActualDate = entity.ActualDate,
            Amount = entity.Amount,
            Description = entity.Description,
            PaymentMethod = entity.PaymentMethod,
            ActionType = actionType,
            ActionByUserId = actionByUserId,
            ActionDate = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    private static IncomeResponseDto ToDto(IncomeDetail i) => new()
    {
        IncomeId = i.IncomeId,
        IncomeCategoryId = i.IncomeCategoryId,
        IncomeCategoryName = i.IncomeCategory?.CategoryName ?? string.Empty,
        IncomeSourceId = i.IncomeSourceId,
        IncomeSourceName = i.IncomeSource?.SourceName,
        ReceivedDate = i.ReceivedDate,
        ActualDate = i.ActualDate,
        Amount = i.Amount,
        Description = i.Description,
        PaymentMethod = i.PaymentMethod,
        CreatedAt = i.CreatedAt,
        LastUpdatedAt = i.LastUpdatedAt
    };
}
