namespace HouseholdExpenseTrackerAPI.DTOs.Category;

public class SubCategoryDto
{
    public int SubCategoryId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class SubCategoryCreateDto
{
    public int CategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
