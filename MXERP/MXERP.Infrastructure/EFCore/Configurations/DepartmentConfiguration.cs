using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Departments;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 部门表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.NumberCode).HasMaxLength(50);
        builder.Property(x => x.Telephone).HasMaxLength(50);
        builder.Property(x => x.Fax).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(800);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_Department_CompanyId");
        builder.HasIndex(x => x.ParentId).HasDatabaseName("IX_Department_ParentId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Department_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Department_Name");
    }
}
