using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Employees;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Employee表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        builder.Property(x => x.BarCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Gender).IsRequired();
        builder.Property(x => x.IsEnable).IsRequired();
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Employee_Name");
        builder.HasIndex(x => x.BarCode).HasDatabaseName("IX_Employee_BarCode");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_Employee_DepartmentId");
    }
}
