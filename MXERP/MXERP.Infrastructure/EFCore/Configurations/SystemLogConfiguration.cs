using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SystemLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SystemLog表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.ToTable("SystemLog");

        builder.Property(x => x.Level).HasMaxLength(50);
        builder.Property(x => x.TimeStamp).HasColumnName("TimeStamp");

        builder.HasIndex(x => x.Level).HasDatabaseName("IX_SystemLog_Level");
        builder.HasIndex(x => x.TimeStamp).HasDatabaseName("IX_SystemLog_TimeStamp");
    }
}
