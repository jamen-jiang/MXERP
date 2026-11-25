using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Works;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 工作日志实体配置
/// </summary>
public class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WorkLog> builder)
    {
        builder.ToTable("WorkLog");

        builder.Property(x => x.TaskName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.HandlerName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.WorkId).HasDatabaseName("IX_WorkLog_WorkId");
        builder.HasIndex(x => x.HandlerId).HasDatabaseName("IX_WorkLog_HandlerId");
    }
}
