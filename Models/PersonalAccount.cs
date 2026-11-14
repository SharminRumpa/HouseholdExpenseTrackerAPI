namespace HouseholdExpenseTrackerAPI.Models
{
    public class PersonalAccount
    {
        public int PersonalAccountId { get; set; }
        public int UserId { get; set; }

        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string? Details { get; set; }

        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PersonalAccountTransaction>
            Transactions
        { get; set; } =
                new List<PersonalAccountTransaction>();
    }
}
