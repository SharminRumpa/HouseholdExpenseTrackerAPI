using HouseholdExpenseTrackerAPI.Data;
using HouseholdExpenseTrackerAPI.DTOs;
using HouseholdExpenseTrackerAPI.DTOs.AccountTransaction;
using HouseholdExpenseTrackerAPI.Models;
using HouseholdExpenseTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using System.Data;
using System.Security.Claims;

public class AccountTransactionService : IAccountTransactionService
{
    private readonly ApplicationDbContext _context;

    public AccountTransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseDto> GetAllAsync(int currentUserId, bool isAdmin, DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from > to)
        {
            return new ResponseDto
            {
                Success = false,
                Message = "'from' date cannot be later than 'to' date."
            };
        }

        var query = _context.PersonalAccountTransactions
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == currentUserId);
        }

        if (from.HasValue)
        {
            var startDate = from.Value.ToDateTime(TimeOnly.MinValue);

            query = query.Where(
                x => x.TransactionDate >= startDate);
        }

        if (to.HasValue)
        {
            var endDateExclusive = to.Value
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue);

            query = query.Where(
                x => x.TransactionDate < endDateExclusive);
        }

        var transactions = await query
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.TransactionId)
            .Select(x => new
            {
                x.TransactionId,
                x.PersonalAccountId,
                x.PersonalAccount.AccountName,
                x.TransactionType,
                x.TransactionDate,
                x.Details,
                x.Debit,
                x.Credit,
                x.BalanceAfterTransaction,
                x.CreatedAt,
                x.CreatedByUserId
            })
            .ToListAsync();

        return new ResponseDto
        {
            Success = true,
            Message = "Account transactions retrieved successfully.",
            Data = transactions
        };
    }

    public async Task<ResponseDto> CreateAsync(
        AccountTransactionCreateDto dto,
        int currentUserId)
    {
        var type = dto.TransactionType.Trim();

        if (!IsValidTransaction(type))
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Invalid transaction type."
            };
        }

        if (dto.Debit < 0 || dto.Credit < 0 ||
            (dto.Debit > 0) == (dto.Credit > 0))
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Provide either a positive Debit or a positive Credit, not both."
            };
        }

        if ((type == "Income" || type == "OpeningBalance")
            && (dto.Debit != 0 || dto.Credit <= 0))
        {
            return new ResponseDto
            {
                Success = false,
                Message = $"{type} transactions must have a positive Credit only."
            };
        }

        if (type == "Expense" &&
            (dto.Credit != 0 || dto.Debit <= 0))
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Expense transactions must have a positive Debit only."
            };
        }

        if (string.IsNullOrWhiteSpace(dto.Details))
        {
            return new ResponseDto
            {
                Success = false,
                Message = "Transaction details are required."
            };
        }

        if (type == "OpeningBalance")
        {
            try
            {
                var exitAccountTransaction = _context.PersonalAccountTransactions
                   .Where(u => u.UserId == currentUserId && u.TransactionType == type && u.IsDeleted == false).FirstOrDefault();

                if(exitAccountTransaction != null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "User OpeningBalance already exits!"
                    };
                    
                }   

            }
            catch
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "Invalid transaction type."
                };
                throw;
            }


            try
            {
                //string CurrentUserName = User.FindFirstValue(ClaimTypes.Name)?? User.FindFirstValue("name" ?? throw new InvalidOperationException("Current user name is not available.");

                string name = _context.Users
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? throw new InvalidOperationException("Current user name is not available.");

                // First time insert data on PersonalAccount table
                var personalAccount = new PersonalAccount
                {
                    UserId = currentUserId,
                    AccountName = name.Trim(),
                    AccountType = "Cash",
                    Details = $"Cash account for {name.Trim()} to manage daily expenses and income. Get real time updates about your financial status (Actual amount).",
                    CurrentBalance = dto.Credit,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                _context.PersonalAccounts.Add(personalAccount);

                await _context.SaveChangesAsync();
            }
            catch
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "Invalid transaction type."
                };
                throw;
            }
        }

        await using var dbTransaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var account = await _context.PersonalAccounts
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.UserId == currentUserId);

            if (account == null)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "Active account not found or access denied."
                };
            }

            var transaction = new PersonalAccountTransaction
            {
                PersonalAccountId = account.PersonalAccountId,
                UserId = account.UserId,
                TransactionType = type,
                TransactionDate = (dto.TransactionDate ??
                    DateOnly.FromDateTime(DateTime.UtcNow))
                    .ToDateTime(TimeOnly.MinValue),
                Details = dto.Details.Trim(),
                Debit = dto.Debit,
                Credit = dto.Credit,
                BalanceAfterTransaction = 0,
                IsDeleted = false,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.PersonalAccountTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            await RecalculateBalanceAsync(account);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new ResponseDto
            {
                Success = true,
                Message = "Account transaction created successfully.",
                Data = new
                {
                    transaction.TransactionId,
                    account.PersonalAccountId,
                    account.CurrentBalance
                }
            };
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseDto> DeleteAsync(
        int transactionId,
        int currentUserId,
        bool isAdmin)
    {
        await using var dbTransaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var query = _context.PersonalAccountTransactions
                .Where(x =>
                    x.TransactionId == transactionId &&
                    !x.IsDeleted);

            if (!isAdmin)
            {
                query = query.Where(
                    x => x.UserId == currentUserId);
            }

            var transaction = await query
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "Active transaction not found or access denied."
                };
            }

            var account = await _context.PersonalAccounts
                .FirstOrDefaultAsync(x =>
                    x.PersonalAccountId ==
                    transaction.PersonalAccountId);

            if (account == null)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "Account not found."
                };
            }

            transaction.IsDeleted = true;

            await _context.SaveChangesAsync();

            await RecalculateBalanceAsync(account);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new ResponseDto
            {
                Success = true,
                Message = "Transaction deleted successfully.",
                Data = new
                {
                    transaction.TransactionId,
                    account.CurrentBalance
                }
            };
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    private async Task RecalculateBalanceAsync(
        PersonalAccount account)
    {
        var transactions = await _context.PersonalAccountTransactions
            .Where(x =>
                x.PersonalAccountId == account.PersonalAccountId &&
                !x.IsDeleted)
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.TransactionId)
            .ToListAsync();

        decimal balance = 0;

        foreach (var item in transactions)
        {
            balance += item.Credit - item.Debit;

            if (balance < 0)
            {
                throw new InvalidOperationException(
                    "This operation would result in a negative account balance.");
            }

            item.BalanceAfterTransaction = balance;
        }

        account.CurrentBalance = balance;
        account.UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidTransaction(string type)
    {
        return type is
            "OpeningBalance" or
            "Income" or
            "Expense" or
            "Adjustment";
    }
}
