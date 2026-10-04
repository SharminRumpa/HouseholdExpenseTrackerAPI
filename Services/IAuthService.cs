using HouseholdExpenseTrackerAPI.DTOs.Auth;

namespace HouseholdExpenseTrackerAPI.Services;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    Task<List<MenuItemDto>> GetMenuForUserAsync(int userId);

}
