namespace HouseholdExpenseTrackerAPI.DTOs.Income;

public class IncomeCreateDto
{
    public int IncomeCategoryId { get; set; }
    public int? IncomeSourceId { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public DateOnly? ActualDate { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }
    public string Month { get; set; } = string.Empty;
}
