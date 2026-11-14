using HouseholdExpenseTrackerAPI.Data; // adjust to your actual DbContext namespace
using HouseholdExpenseTrackerAPI.DTOs.Report;
using ClosedXML.Excel; // NuGet: ClosedXML
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;

        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ExpenseReportDto> GetExpenseReportAsync(
            DateTime? from, DateTime? to, int? categoryId, int? subCategoryId, string? expenseBy, string? expenseFor)
        {
            var query = _db.ExpenseDetails.AsNoTracking().AsQueryable();

            // ExpenseDate is a DATE column, so both ends are inclusive whole days.
            //if (from.HasValue) query = query.Where(e => e.ExpenseDate >= from.Value.Date);

            if (from.HasValue)
                query = query.Where(e => e.ExpenseDate >= DateOnly.FromDateTime(from.Value));

            if (to.HasValue)
                query = query.Where(e => e.ExpenseDate <= DateOnly.FromDateTime(to.Value));

            //if (to.HasValue) query = query.Where(e => e.ExpenseDate <= to.Value.Date);
            if (categoryId.HasValue) query = query.Where(e => e.ExpenseCategoryId == categoryId.Value);
            if (subCategoryId.HasValue) query = query.Where(e => e.ExpenseSubCategoryId == subCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(expenseBy))
            {
                var person = expenseBy.Trim();
                query = query.Where(e => e.ExpenseBy == person);
            }

            if (!string.IsNullOrWhiteSpace(expenseFor))
            {
                var forPerson = expenseFor.Trim();
                query = query.Where(e => e.ExpenseFor == forPerson);
            }

            // Joins instead of navigation properties, so this doesn't depend on how
            // the ExpenseDetail entity is wired up. Sub-category and item are optional.
            var rows = await (
                from e in query
                join c in _db.ExpenseCategories on e.ExpenseCategoryId equals c.ExpenseCategoryId
                join s in _db.ExpenseSubCategories on e.ExpenseSubCategoryId equals s.ExpenseSubCategoryId into subs
                from s in subs.DefaultIfEmpty()
                join i in _db.ExpenseItems on e.ExpenseItemId equals i.ExpenseItemId into items
                from i in items.DefaultIfEmpty()
                orderby e.ExpenseDate descending, e.ExpenseId descending
                select new ExpenseReportRowDto
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseDate = e.ExpenseDate,
                    CategoryName = c.CategoryName,
                    SubCategoryName = s != null ? s.SubCategoryName : null,
                    ItemName = i != null ? i.ItemName : null,
                    Description = e.Description,
                    ExpenseBy = e.ExpenseBy,
                    ExpenseFor = e.ExpenseFor,
                    Amount = e.Amount
                }).ToListAsync();

            return new ExpenseReportDto
            {
                TotalAmount = rows.Sum(r => r.Amount),
                RecordCount = rows.Count,
                Rows = rows
            };
        }

        public async Task<byte[]> ExportExpenseReportAsync(
            DateTime? from, DateTime? to, int? categoryId, int? subCategoryId, string? expenseBy, string? expenseFor)
        {
            var report = await GetExpenseReportAsync(from, to, categoryId, subCategoryId, expenseBy, expenseFor);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Expense Report");

            string period = $"{(from.HasValue ? from.Value.ToString("dd MMM yyyy") : "Start")} to " +
                            $"{(to.HasValue ? to.Value.ToString("dd MMM yyyy") : "Today")}";

            sheet.Cell(1, 1).Value = "Expense Report";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 14;
            sheet.Cell(2, 1).Value = $"Period: {period}";

            string[] headers =
            {
                "#", "Date", "Category", "Sub Category", "Item", "Description", "Expense By", "Expense For", "Amount"
            };

            const int headerRow = 4;
            for (int c = 0; c < headers.Length; c++)
            {
                sheet.Cell(headerRow, c + 1).Value = headers[c];
            }

            var header = sheet.Range(headerRow, 1, headerRow, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = headerRow + 1;
            int number = 1;
            foreach (var r in report.Rows)
            {
                sheet.Cell(row, 1).Value = number++;
                sheet.Cell(row, 2).Value = r.ExpenseDate.Day;
                sheet.Cell(row, 3).Value = r.CategoryName;
                sheet.Cell(row, 4).Value = r.SubCategoryName ?? "";
                sheet.Cell(row, 5).Value = r.ItemName ?? "";
                sheet.Cell(row, 6).Value = r.Description ?? "";
                sheet.Cell(row, 7).Value = r.ExpenseBy ?? "";
                sheet.Cell(row, 8).Value = r.ExpenseFor ?? "";
                sheet.Cell(row, 9).Value = r.Amount;
                row++;
            }

            sheet.Range(headerRow + 1, 2, row - 1, 2).Style.DateFormat.Format = "dd MMM yyyy";
            sheet.Range(headerRow + 1, 9, row, 9).Style.NumberFormat.Format = "#,##0.00";

            // Total row: a real SUM so it stays correct if the user edits the sheet.
            sheet.Cell(row, 8).Value = "Total";
            sheet.Cell(row, 8).Style.Font.Bold = true;
            if (report.Rows.Count > 0)
            {
                sheet.Cell(row, 9).FormulaA1 = $"SUM(I{headerRow + 1}:I{row - 1})";
            }
            else
            {
                sheet.Cell(row, 9).Value = 0;
            }
            sheet.Cell(row, 9).Style.Font.Bold = true;

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        
    }
}
