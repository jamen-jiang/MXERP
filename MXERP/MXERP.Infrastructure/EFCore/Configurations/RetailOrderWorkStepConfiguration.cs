using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.RetailOrderWorkSteps;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// RetailOrderWorkStep表映射
/// </summary>
public sealed class RetailOrderWorkStepConfiguration : IEntityTypeConfiguration<RetailOrderWorkStep>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RetailOrderWorkStep> builder)
    {
        builder.ToTable("RetailOrderWorkStep");

        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Sequence).HasDatabaseName("IX_RetailOrderWorkStep_Sequence");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_RetailOrderWorkStep_DepartmentId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_RetailOrderWorkStep_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_RetailOrderWorkStep_Name");
    }
}
