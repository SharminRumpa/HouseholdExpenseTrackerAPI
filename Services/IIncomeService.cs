using HouseholdExpenseTrackerAPI.DTOs.Income;

namespace HouseholdExpenseTrackerAPI.Services;

public interface IIncomeService
{
    Task<List<IncomeResponseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? categoryId);
    Task<IncomeResponseDto?> GetByIdAsync(int id);
    Task<IncomeResponseDto> CreateAsync(IncomeCreateDto dto, int? currentUserId);
    Task<IncomeResponseDto?> UpdateAsync(int id, IncomeUpdateDto dto, int? currentUserId);
    Task<bool> DeleteAsync(int id, int? currentUserId);
}
