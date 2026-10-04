namespace HouseholdExpenseTrackerAPI.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(int year, int month);
}

public class DashboardSummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public List<CategoryTotalDto> ExpenseByCategory { get; set; } = new();
    public List<CategoryTotalDto> IncomeByCategory { get; set; } = new();
}

public class CategoryTotalDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
