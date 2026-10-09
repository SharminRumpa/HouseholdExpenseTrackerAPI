using HouseholdExpenseTrackerAPI.DTOs;
using HouseholdExpenseTrackerAPI.DTOs.Expense;

namespace HouseholdExpenseTrackerAPI.Services;

public interface IExpenseService
{
    Task<List<ExpenseResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId);
    Task<ExpenseResponseDto?> GetByIdAsync(int id);
    Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto, int? currentUserId);
    Task<ExpenseResponseDto?> UpdateAsync(int id, ExpenseUpdateDto dto, int? currentUserId);
    Task<bool> DeleteAsync(int id, int? currentUserId);

    #region ExpenseDeleteRequests

    Task<ResponseDto> CreateDeleteRequestAsync(DeleteRequestDto dto, int? currentUserId);

    Task<ResponseDto> GetPendingDeleteRequestsAsync(int? currentUserId);

    Task<ResponseDto> ApproveDeleteRequestAsync(int requestId, DeleteReviewDto dto, int? currentUserId);

    Task<ResponseDto> RejectDeleteRequestAsync(int requestId,DeleteReviewDto dto,int? currentUserId);


    #endregion


    
}
