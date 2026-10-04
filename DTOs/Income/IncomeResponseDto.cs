namespace HouseholdExpenseTrackerAPI.DTOs.Income;

public class IncomeResponseDto
{
    public int IncomeId { get; set; }
    public int IncomeCategoryId { get; set; }
    public string IncomeCategoryName { get; set; } = string.Empty;
    public int? IncomeSourceId { get; set; }
    public string? IncomeSourceName { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public DateOnly? ActualDate { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int Year { get; set; }
    public string Month { get; set; } = string.Empty;
}
