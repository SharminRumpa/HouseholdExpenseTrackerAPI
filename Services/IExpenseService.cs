using HouseholdExpenseTrackerAPI.DTOs.Expense;

namespace HouseholdExpenseTrackerAPI.Services;

public interface IExpenseService
{
    Task<List<ExpenseResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId);
    Task<ExpenseResponseDto?> GetByIdAsync(int id);
    Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto, int? currentUserId);
    Task<ExpenseResponseDto?> UpdateAsync(int id, ExpenseUpdateDto dto, int? currentUserId);
    Task<bool> DeleteAsync(int id, int? currentUserId);
}
