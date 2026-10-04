using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs.Auth;
using HouseholdExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseholdExpenseTrackerAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(ApplicationDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("A user with this email already exists.");

        var roleName = string.IsNullOrWhiteSpace(request.RoleName) ? "Member" : request.RoleName;
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName)
            ?? throw new InvalidOperationException($"Role '{roleName}' does not exist.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            MobileNumber = request.MobileNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return GenerateLoginResponse(user, role.RoleName);
    }


    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user is null)
        {
            return null;
        }

        // Requires the BCrypt.Net-Next NuGet package. PasswordHash must have
        // been written with BCrypt.HashPassword(...) when the user was created
        // — see the note at the end about existing/seeded users.
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var roleName = user.Role?.RoleName ?? string.Empty;
        var (token, expiresAt) = _tokenService.GenerateToken(user.UserId, user.Name, user.Email, roleName);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            RoleName = roleName
        };
    }



    public async Task<List<MenuItemDto>> GetMenuForUserAsync(int userId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        if (user is null)
        {
            return new List<MenuItemDto>();
        }

        // Only permissions with a Route are navigable menu entries —
        // a role can hold a permission that gates an action (e.g. "can
        // delete expenses") without it ever appearing in the sidebar.
        return await _db.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == user.RoleId)
            .Select(rp => rp.MenuDetail!)
            .Where(m => m.Route != null && m.IsActive)
            .OrderBy(m => m.OrderBy)
            .Select(m => new MenuItemDto
            {
                Id = m.MenuDetailsId,
                Title = m.MenuName,
                Icon = m.Icon,
                Route = m.Route,
                Permissions = new List<string> { m.MenuCode }
            })
            .ToListAsync();
    }

    private LoginResponseDto GenerateLoginResponse(User user, string roleName)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(user.UserId, user.Name, user.Email, roleName);
        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            RoleName = roleName
        };
    }
}


