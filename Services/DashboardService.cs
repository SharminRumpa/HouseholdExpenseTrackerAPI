using HouseholdExpenseTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);

        var incomeQuery = _db.IncomeDetails
            .Include(i => i.IncomeCategory)
            .Where(i => i.ReceivedDate >= start && i.ReceivedDate < end);

        var expenseQuery = _db.ExpenseDetails
            .Include(e => e.ExpenseCategory)
            .Where(e => e.ExpenseDate >= start && e.ExpenseDate < end);

        var totalIncome = await incomeQuery.SumAsync(i => (decimal?)i.Amount) ?? 0m;
        var totalExpense = await expenseQuery.SumAsync(e => (decimal?)e.Amount) ?? 0m;

        var expenseByCategory = await expenseQuery
            .GroupBy(e => e.ExpenseCategory!.CategoryName)
            .Select(g => new CategoryTotalDto { CategoryName = g.Key, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        var incomeByCategory = await incomeQuery
            .GroupBy(i => i.IncomeCategory!.CategoryName)
            .Select(g => new CategoryTotalDto { CategoryName = g.Key, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return new DashboardSummaryDto
        {
            Year = year,
            Month = month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = totalIncome - totalExpense,
            ExpenseByCategory = expenseByCategory,
            IncomeByCategory = incomeByCategory
        };
    }
}
