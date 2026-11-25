using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Works;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 工作实体配置
/// </summary>
public class WorkConfiguration : IEntityTypeConfiguration<Work>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Work> builder)
    {
        builder.ToTable("Work");

        builder.Property(x => x.WorkflowId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.BusinessType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.BusinessNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CurrentTask).HasMaxLength(50);
        builder.Property(x => x.CurrentTaskAssigner).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();

        builder.HasIndex(x => x.WorkflowId).HasDatabaseName("IX_Work_WorkflowId");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Work_Name");
        builder.HasIndex(x => x.BusinessType).HasDatabaseName("IX_Work_BusinessType");
        builder.HasIndex(x => x.BusinessId).HasDatabaseName("IX_Work_BusinessId");
        builder.HasIndex(x => x.BusinessNumber).HasDatabaseName("IX_Work_BusinessNumber");
        builder.HasIndex(x => x.CurrentTaskAssigner).HasDatabaseName("IX_Work_CurrentTaskAssigner");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Work_Status");

        builder.HasMany(x => x.WorkLogs)
            .WithOne()
            .HasForeignKey(x => x.WorkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
