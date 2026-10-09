using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs;
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

    #region Expense

    public async Task<List<ExpenseResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId)
    {
        var query = _db.ExpenseDetails
            .Include(e => e.ExpenseCategory)
            .Include(e => e.ExpenseSubCategory)
            .Include(e => e.ExpenseItem)
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
            CreatedByUserId = currentUserId.Value,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            LastUpdatedByUserId = currentUserId.Value
        };

        // Everything between BeginTransactionAsync and CommitAsync is one unit:
        // the ExpenseDetails insert AND whatever LogAsync writes internally.
        // If LogAsync throws, the catch below rolls both back — the expense
        // row never ends up committed without its log entry.
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.ExpenseDetails.Add(entity);
            await _db.SaveChangesAsync();

            await LogAsync(entity, "INSERT", currentUserId);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        // Read-back happens after commit, outside the transaction — it's just
        // fetching what was already durably saved.
        return await GetByIdAsync(entity.ExpenseId) ?? ToDto(entity);
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, ExpenseUpdateDto dto, int? currentUserId)
    {
        var entity = await _db.ExpenseDetails.FirstOrDefaultAsync(e => e.ExpenseId == id);
        if (entity is null) return null;
        if (currentUserId is null) throw new ArgumentNullException(nameof(currentUserId));

        entity.ExpenseCategoryId = dto.ExpenseCategoryId;
        entity.ExpenseSubCategoryId = dto.ExpenseSubCategoryId;
        entity.ExpenseItemId = dto.ExpenseItemId;
        entity.ExpenseDate = dto.ExpenseDate;
        entity.Amount = dto.Amount;
        entity.Quantity = dto.Quantity;
        entity.Unit = dto.Unit;
        entity.NetWeight = dto.NetWeight;
        entity.WeightUnit = dto.WeightUnit;
        entity.PaymentMethod = dto.PaymentMethod;
        entity.ExpenseBy = dto.ExpenseBy;
        entity.ExpenseFor = dto.ExpenseFor;
        entity.Description = dto.Description;
        entity.LastUpdatedByUserId = currentUserId.Value;
        entity.LastUpdatedAt = DateTime.UtcNow;

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.SaveChangesAsync();

            await LogAsync(entity, "UPDATE", currentUserId);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

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
        ExpenseSubCategoryName = e.ExpenseSubCategory?.SubCategoryName ?? string.Empty,
        ExpenseItemId = e.ExpenseItemId,
        ExpenseItemName = e.ExpenseItem?.ItemName ?? string.Empty,
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

    #endregion


    #region Expense Delete Requests

    public async Task<ResponseDto> CreateDeleteRequestAsync(
    DeleteRequestDto dto,
    int? currentUserId)
    {
        // Validation
        var expense = await _db.ExpenseDetails
            .FirstOrDefaultAsync(x => x.ExpenseId == dto.Id);

        if (expense == null)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Expense not found."
            };
        }

        // Prevent duplicate pending request
        var exists = await _db.ExpenseDeleteRequests
            .AnyAsync(x =>
                x.ExpenseId == dto.Id &&
                x.Status == "Pending");

        if (exists)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "A delete request is already pending."
            };
        }

        var request = new ExpenseDeleteRequest
        {
            ExpenseId = dto.Id,
            RequestedByUserId = currentUserId.Value,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _db.ExpenseDeleteRequests.Add(request);

        await _db.SaveChangesAsync();

        return new ResponseDto
        {
            Success = true,
            Message = "Expense delete request submitted successfully."
        };
    }


    // =====================================================
    // Get Pending Delete Requests
    // =====================================================

    public async Task<ResponseDto> GetPendingDeleteRequestsAsync(int? currentUserId)
    {
        var requests = await _db.ExpenseDeleteRequests
            .AsNoTracking()
            .Include(x => x.Expense)
            .Include(x => x.RequestedByUser)
            .Where(x => x.Status == "Pending")
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.ExpenseDeleteRequestId,
                x.ExpenseId,
                x.Reason,
                x.Status,
                x.CreatedAt,

                RequestedBy = x.RequestedByUser!.Name,

                Expense = x.Expense == null
                    ? null
                    : new
                    {
                        x.Expense.ExpenseId,
                        x.Expense.ExpenseDate,
                        x.Expense.Amount,
                        x.Expense.ExpenseBy,
                        x.Expense.ExpenseFor,
                        x.Expense.Description
                    }
            })
            .ToListAsync();

        return new ResponseDto
        {
            Success = true,
            Message = "Pending delete requests retrieved successfully.",
            Data = requests
        };
    }

    // =====================================================
    // Approve Delete Request
    // =====================================================

    public async Task<ResponseDto> ApproveDeleteRequestAsync(
        int requestId,
        DeleteReviewDto dto,
        int? currentUserId)
    {
        var request = await _db.ExpenseDeleteRequests
            .FirstOrDefaultAsync(x =>
                x.ExpenseDeleteRequestId == requestId &&
                x.Status == "Pending");

        if (request == null)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Pending delete request not found."
            };
        }

        if (request.ExpenseId == null)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Expense no longer exists."
            };
        }

        var expense = await _db.ExpenseDetails
            .FirstOrDefaultAsync(x =>
                x.ExpenseId == request.ExpenseId);

        if (expense == null)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Expense not found."
            };
        }

        request.Status = "Approved";
        request.ReviewedByUserId = currentUserId;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewComment = dto.ReviewComment;

        _db.ExpenseDetails.Remove(expense);

        await _db.SaveChangesAsync();

        return new ResponseDto
        {
            Success = true,
            Message = "Expense delete request approved. Expense deleted successfully."
        };
    }

    // =====================================================
    // Reject Delete Request
    // =====================================================

    public async Task<ResponseDto> RejectDeleteRequestAsync(
        int requestId,
        DeleteReviewDto dto,
        int? currentUserId)
    {
        var request = await _db.ExpenseDeleteRequests
            .FirstOrDefaultAsync(x =>
                x.ExpenseDeleteRequestId == requestId &&
                x.Status == "Pending");

        if (request == null)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Pending delete request not found."
            };
        }

        request.Status = "Rejected";
        request.ReviewedByUserId = currentUserId;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewComment = dto.ReviewComment;

        await _db.SaveChangesAsync();

        return new ResponseDto
        {
            Success = true,
            Message = "Expense delete request rejected."
        };
    }


    #endregion

  
}
