namespace HouseholdExpenseTrackerAPI.DTOs.Report
{
    public class ReportDto
    {
    }
    public class ExpenseReportRowDto
    {
        public int ExpenseId { get; set; }
        public DateOnly ExpenseDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? SubCategoryName { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public string? ExpenseBy { get; set; }
        public string? ExpenseFor { get; set; }
        public decimal Amount { get; set; }
    }

    public class ExpenseReportDto
    {
        // Total of every row matching the filters (not just one page).
        public decimal TotalAmount { get; set; }
        public int RecordCount { get; set; }
        public List<ExpenseReportRowDto> Rows { get; set; } = new();
    }
}
