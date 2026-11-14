namespace HouseholdExpenseTrackerAPI.Models
{
    public class PersonalAccountTransaction
    {
        public int TransactionId { get; set; }
        public int PersonalAccountId { get; set; }
        public int UserId { get; set; }

        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string Details { get; set; } = string.Empty;

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal BalanceAfterTransaction { get; set; }

        public bool IsDeleted { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public PersonalAccount? PersonalAccount { get; set; }
        public User? User { get; set; }
        public User? CreatedByUser { get; set; }
    }
}
