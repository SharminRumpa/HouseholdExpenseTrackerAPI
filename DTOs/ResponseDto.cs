namespace HouseholdExpenseTrackerAPI.DTOs
{
    public class ResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public dynamic Data { get; set; } = null;
    }

    public class DeleteRequestDto
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
    }

    public class DeleteReviewDto
    {
        public string? ReviewComment { get; set; }
    }
}
