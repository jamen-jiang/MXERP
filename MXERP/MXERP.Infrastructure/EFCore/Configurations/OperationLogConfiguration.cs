using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.OperationLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// OperationLog表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("OperationLog");

        builder.Property(x => x.TraceId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ApiModule).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ApiName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ApiUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ApiMethod).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Ip).HasMaxLength(200).HasColumnName("IP");
        builder.Property(x => x.City).HasMaxLength(50);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.Browser).HasMaxLength(200);
        builder.Property(x => x.Os).HasMaxLength(200);

        builder.HasIndex(x => x.TraceId).HasDatabaseName("IX_OperationLog_TraceId");
        builder.HasIndex(x => x.ApiModule).HasDatabaseName("IX_OperationLog_ApiModule");
        builder.HasIndex(x => x.ApiName).HasDatabaseName("IX_OperationLog_ApiName");
        builder.HasIndex(x => x.ApiUrl).HasDatabaseName("IX_OperationLog_ApiUrl");
    }
}
