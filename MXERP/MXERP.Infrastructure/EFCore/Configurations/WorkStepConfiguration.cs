using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.WorkSteps;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 工序表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class WorkStepConfiguration : IEntityTypeConfiguration<WorkStep>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WorkStep> builder)
    {
        builder.ToTable("WorkStep");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Sequence).HasDatabaseName("IX_WorkStep_Sequence");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_WorkStep_DepartmentId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_WorkStep_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_WorkStep_Name");
    }
}
