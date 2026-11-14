using System.ComponentModel.DataAnnotations;

namespace HouseholdExpenseTrackerAPI.DTOs.AccountTransaction
{
    public class AccountTransaction
    {
    }

    public class AccountTransactionCreateDto
    {
        [Range(1, int.MaxValue)]
        public int PersonalAccountId { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        public DateOnly? TransactionDate { get; set; }

        [Required]
        [MaxLength(250)]
        public string Details { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.00", "9999999999999999.99")]
        public decimal Debit { get; set; }

        [Range(typeof(decimal), "0.00", "9999999999999999.99")]
        public decimal Credit { get; set; }
    }
}
