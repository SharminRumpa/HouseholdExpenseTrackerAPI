using HouseholdExpenseTrackerAPI.DTOs;
using HouseholdExpenseTrackerAPI.DTOs.AccountTransaction;

namespace HouseholdExpenseTrackerAPI.Services.Interfaces
{
    public interface IAccountTransactionService
    {
        Task<ResponseDto> GetAllAsync(
        int currentUserId,
        bool isAdmin,
        DateOnly? from,
        DateOnly? to);

        Task<ResponseDto> CreateAsync(
            AccountTransactionCreateDto dto,
            int currentUserId);

        Task<ResponseDto> DeleteAsync(
            int transactionId,
            int currentUserId,
            bool isAdmin);
    }
}
