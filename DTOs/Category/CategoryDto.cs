namespace HouseholdExpenseTrackerAPI.DTOs.Category;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryCreateDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CategoryUpdateDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class IncomeSourceDto
{
    public int IncomeSourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class IncomeSourceCreateDto
{
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class IncomeSourceUpdateDto
{
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
