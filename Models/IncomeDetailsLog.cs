namespace HouseholdExpenseTrackerAPI.Models;

public class IncomeDetailsLog
{
    public long IncomeLogId { get; set; }

    public int IncomeId { get; set; }
    public int? IncomeCategoryId { get; set; }
    public int? IncomeSourceId { get; set; }

    public DateOnly? ReceivedDate { get; set; }
    public DateOnly? ActualDate { get; set; }

    public decimal? Amount { get; set; }
    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }

    public string ActionType { get; set; } = string.Empty; // INSERT / UPDATE / DELETE
    public int? ActionByUserId { get; set; }
    public DateTime ActionDate { get; set; }
    public int? Year { get; set; }
    public string? Month { get; set; }
}
