using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Jobs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 任务表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Job");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.BeanName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.MethodName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Cron).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.IsEnable).IsRequired();

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Job_Name");
    }
}
