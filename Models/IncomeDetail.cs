namespace HouseholdExpenseTrackerAPI.Models;

public class IncomeDetail
{
    public int IncomeId { get; set; }

    public int IncomeCategoryId { get; set; }
    public IncomeCategory? IncomeCategory { get; set; }

    public int? IncomeSourceId { get; set; }
    public IncomeSource? IncomeSource { get; set; }

    public DateOnly ReceivedDate { get; set; }
    public DateOnly? ActualDate { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? LastUpdatedByUserId { get; set; }
    public User? LastUpdatedByUser { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int Year { get; set; }
    public string Month { get; set; } = string.Empty;  
}
