using HouseholdExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseholdExpenseTrackerAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // =========================
    // Security
    // =========================

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<MenuDetail> MenuDetails => Set<MenuDetail>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // =========================
    // Income
    // =========================

    public DbSet<IncomeCategory> IncomeCategories => Set<IncomeCategory>();
    public DbSet<IncomeSource> IncomeSources => Set<IncomeSource>();
    public DbSet<IncomeDetail> IncomeDetails => Set<IncomeDetail>();
    public DbSet<IncomeDetailsLog> IncomeDetailsLogs => Set<IncomeDetailsLog>();

    // =========================
    // Expense
    // =========================

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<ExpenseSubCategory> ExpenseSubCategories => Set<ExpenseSubCategory>();
    public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
    public DbSet<ExpenseDetail> ExpenseDetails => Set<ExpenseDetail>();
    public DbSet<ExpenseDetailsLog> ExpenseDetailsLogs => Set<ExpenseDetailsLog>();
    public virtual DbSet<ExpenseDeleteRequest> ExpenseDeleteRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureMenuDetail(modelBuilder);
        ConfigureRolePermission(modelBuilder);

        ConfigureIncomeCategory(modelBuilder);
        ConfigureIncomeSource(modelBuilder);
        ConfigureIncomeDetail(modelBuilder);
        ConfigureIncomeDetailsLog(modelBuilder);

        ConfigureExpenseCategory(modelBuilder);
        ConfigureExpenseSubCategory(modelBuilder);
        ConfigureExpenseItem(modelBuilder);
        ConfigureExpenseDetail(modelBuilder);
        ConfigureExpenseDetailsLog(modelBuilder);
    }

    // =========================
    // User
    // =========================

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20);

            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.RoleId)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedBy);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.UpdatedBy);

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing audit fields.
            // CreatedBy/UpdatedBy are nullable for the first/root user.
            //entity.HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(e => e.CreatedBy)
            //    .OnDelete(DeleteBehavior.Restrict);

            //entity.HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(e => e.UpdatedBy)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Created By
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Updated By
            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }

    // =========================
    // Role
    // =========================

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => e.RoleName)
                .IsUnique();
        });
    }

    // =========================
    // Menu Detail
    // =========================

    private static void ConfigureMenuDetail(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuDetail>(entity =>
        {
            entity.ToTable("MenuDetails");

            entity.HasKey(e => e.MenuDetailsId);

            entity.Property(e => e.MenuDetailsId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MenuCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.MenuName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Route)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(e => e.Icon)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.OrderBy)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => e.MenuCode)
                .IsUnique();
        });
    }

    // =========================
    // Role Permission
    // =========================

    private static void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");

            entity.HasKey(e => e.RolePermissionId);

            entity.Property(e => e.RolePermissionId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => new
            {
                e.RoleId,
                e.MenuDetailsId
            })
            .IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.MenuDetail)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.MenuDetailsId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // =========================
    // Income Category
    // =========================

    private static void ConfigureIncomeCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IncomeCategory>(entity =>
        {
            entity.ToTable("IncomeCategories");

            entity.HasKey(e => e.IncomeCategoryId);

            entity.Property(e => e.IncomeCategoryId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => e.CategoryName)
                .IsUnique();
        });
    }

    // =========================
    // Income Source
    // =========================

    private static void ConfigureIncomeSource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IncomeSource>(entity =>
        {
            entity.ToTable("IncomeSources");

            entity.HasKey(e => e.IncomeSourceId);

            entity.Property(e => e.IncomeSourceId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SourceName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => e.SourceName)
                .IsUnique();
        });
    }

    // =========================
    // Income Detail
    // =========================

    private static void ConfigureIncomeDetail(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IncomeDetail>(entity =>
        {
            entity.ToTable("IncomeDetails");

            entity.HasKey(e => e.IncomeId);

            entity.Property(e => e.IncomeId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ReceivedDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.ActualDate)
                .HasColumnType("date");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);

            entity.Property(e => e.CreatedByUserId);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedByUserId);

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.Year)
            .HasComputedColumnSql("YEAR(ReceivedDate)", stored: true)
            .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.Month)
                .HasComputedColumnSql(MonthCaseExpressionSql, stored: true)
                .HasMaxLength(9)
                .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(e => e.IncomeCategory)
                .WithMany(e => e.IncomeDetails)
                .HasForeignKey(e => e.IncomeCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IncomeSource)
                .WithMany(e => e.IncomeDetails)
                .HasForeignKey(e => e.IncomeSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Created By
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Updated By
            entity.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


        });

    }

    // =========================
    // Income Details Log
    // =========================

    private static void ConfigureIncomeDetailsLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IncomeDetailsLog>(entity =>
        {
            entity.ToTable("IncomeDetailsLog");

            entity.HasKey(e => e.IncomeLogId);

            entity.Property(e => e.IncomeLogId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ReceivedDate)
                .HasColumnType("date");

            entity.Property(e => e.ActualDate)
                .HasColumnType("date");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);

            entity.Property(e => e.ActionType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime2");

            entity.Property(e => e.Year)
            .HasComputedColumnSql("YEAR(ReceivedDate)", stored: true)
            .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.Month)
                .HasMaxLength(9)
                .HasComputedColumnSql(MonthCaseExpressionSql, stored: true)
                .ValueGeneratedOnAddOrUpdate();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.ActionByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // =========================
    // Expense Category
    // =========================

    private static void ConfigureExpenseCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.ToTable("ExpenseCategories");

            entity.HasKey(e => e.ExpenseCategoryId);

            entity.Property(e => e.ExpenseCategoryId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => e.CategoryName)
                .IsUnique();
        });
    }

    // =========================
    // Expense SubCategory
    // =========================

    private static void ConfigureExpenseSubCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseSubCategory>(entity =>
        {
            entity.ToTable("ExpenseSubCategories");

            entity.HasKey(e => e.ExpenseSubCategoryId);

            entity.Property(e => e.ExpenseSubCategoryId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SubCategoryName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => new
            {
                e.ExpenseCategoryId,
                e.SubCategoryName
            })
            .IsUnique();

            //entity.HasOne(e => e.ExpenseCategory)
            //    .WithMany(e => e.ExpenseSubCategories)
            //    .HasForeignKey(e => e.ExpenseCategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ExpenseCategory)
                .WithMany(e => e.SubCategories) // <-- use actual property name
                    .HasForeignKey(e => e.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }


    // =========================
    // Expense Item (third layer)
    // =========================

    private static void ConfigureExpenseItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseItem>(entity =>
        {
            entity.ToTable("ExpenseItems");

            entity.HasKey(e => e.ExpenseItemId);

            entity.Property(e => e.ExpenseItemId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasIndex(e => new
            {
                e.ExpenseSubCategoryId,
                e.ItemName
            })
            .IsUnique();

            entity.HasOne(e => e.ExpenseSubCategory)
                .WithMany()
                .HasForeignKey(e => e.ExpenseSubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // =========================
    // Expense Detail
    // =========================

    private static void ConfigureExpenseDetail(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseDetail>(entity =>
        {
            entity.ToTable("ExpenseDetails");

            entity.HasKey(e => e.ExpenseId);

            entity.Property(e => e.ExpenseId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ExpenseDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .HasPrecision(18, 3);

            entity.Property(e => e.Unit)
                .HasMaxLength(30);

            entity.Property(e => e.NetWeight)
                .HasPrecision(18, 3);

            entity.Property(e => e.WeightUnit)
                .HasMaxLength(20);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);

            entity.Property(e => e.ExpenseBy)
                .HasMaxLength(50);

            entity.Property(e => e.ExpenseFor)
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedByUserId);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedByUserId);

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnType("datetime2");

            entity.HasOne(e => e.ExpenseCategory)
                .WithMany(e => e.ExpenseDetails)
                .HasForeignKey(e => e.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ExpenseSubCategory)
                .WithMany(e => e.ExpenseDetails)
                .HasForeignKey(e => e.ExpenseSubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ExpenseItem)
                .WithMany()
                .HasForeignKey(e => e.ExpenseItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Created By
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Updated By
            entity.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


        });
    }

    // =========================
    // Expense Details Log
    // =========================

    private static void ConfigureExpenseDetailsLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseDetailsLog>(entity =>
        {
            entity.ToTable("ExpenseDetailsLog");

            entity.HasKey(e => e.ExpenseLogId);

            entity.Property(e => e.ExpenseLogId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ExpenseDate)
                .HasColumnType("date");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            entity.Property(e => e.Quantity)
                .HasPrecision(18, 3);

            entity.Property(e => e.Unit)
                .HasMaxLength(30);

            entity.Property(e => e.NetWeight)
                .HasPrecision(18, 3);

            entity.Property(e => e.WeightUnit)
                .HasMaxLength(20);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);

            entity.Property(e => e.ExpenseBy)
                .HasMaxLength(50);

            entity.Property(e => e.ExpenseFor)
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.ActionType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime2");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.ActionByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // =========================
    // Expense Delete Request
    private static void ConfigureExpenseDeleteRequest(ModelBuilder modelBuilder)
    {
       
        // =========================

        modelBuilder.Entity<ExpenseDeleteRequest>(entity =>
        {
            entity.ToTable("ExpenseDeleteRequests");

            entity.HasKey(e => e.ExpenseDeleteRequestId);

            entity.Property(e => e.ExpenseDeleteRequestId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Reason)
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ReviewComment)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.ReviewedAt)
                .HasColumnType("datetime2");

            entity.HasOne(e => e.Expense)
                .WithMany()
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RequestedByUser)
                .WithMany()
                .HasForeignKey(e => e.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReviewedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ExpenseId);

            entity.HasIndex(e => e.Status);

            entity.HasIndex(e => e.RequestedByUserId);
        });
    }

    private const string MonthCaseExpressionSql = """
        CASE MONTH(ReceivedDate)
            WHEN 1  THEN 'January'
            WHEN 2  THEN 'February'
            WHEN 3  THEN 'March'
            WHEN 4  THEN 'April'
            WHEN 5  THEN 'May'
            WHEN 6  THEN 'June'
            WHEN 7  THEN 'July'
            WHEN 8  THEN 'August'
            WHEN 9  THEN 'September'
            WHEN 10 THEN 'October'
            WHEN 11 THEN 'November'
            WHEN 12 THEN 'December'
        END
        """;
}
