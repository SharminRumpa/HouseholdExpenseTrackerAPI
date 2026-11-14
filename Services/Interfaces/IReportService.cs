using HouseholdExpenseTrackerAPI.DTOs.Report;

namespace HouseholdExpenseTrackerAPI.Services
{
    public interface IReportService
    {
        Task<ExpenseReportDto> GetExpenseReportAsync(
            DateTime? from, DateTime? to, int? categoryId, int? subCategoryId, string? expenseBy, string? expenseFor);

        // Same filters as the report; returns an .xlsx file as bytes.
        Task<byte[]> ExportExpenseReportAsync(
            DateTime? from, DateTime? to, int? categoryId, int? subCategoryId, string? expenseBy, string? expenseFor);

        
    }
}
