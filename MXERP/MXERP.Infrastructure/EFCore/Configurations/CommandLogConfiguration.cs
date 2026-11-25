using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.CommandLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// CommandLog表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class CommandLogConfiguration : IEntityTypeConfiguration<CommandLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CommandLog> builder)
    {
        builder.ToTable("CommandLog");

        builder.Property(x => x.TraceId).HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TypeName).IsRequired().HasMaxLength(200);

        builder.HasIndex(x => x.TraceId).HasDatabaseName("IX_CommandLog_TraceId");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_CommandLog_Name");
        builder.HasIndex(x => x.TypeName).HasDatabaseName("IX_CommandLog_TypeName");
    }
}
