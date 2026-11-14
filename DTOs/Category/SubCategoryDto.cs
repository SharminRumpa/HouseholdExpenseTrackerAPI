namespace HouseholdExpenseTrackerAPI.DTOs.Category;

public class SubCategoryDto
{
    public int SubCategoryId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

public class ExpenseSubCategoryResponseDto
{
    public int ExpenseSubCategoryId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

public class SubCategoryCreateDto
{
    public int CategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Used for both create (POST) and update (PUT).
public class ExpenseSubCategorySaveDto
{
    public int ExpenseCategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

// Body of PATCH api/expense-sub-categories/{id}/status
public class ExpenseSubCategoryStatusDto
{
    public bool IsActive { get; set; }
}

