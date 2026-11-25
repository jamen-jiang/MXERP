using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExternalOrderLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 外部订单日志配置
/// </summary>
public sealed class ExternalOrderLogConfiguration : IEntityTypeConfiguration<ExternalOrderLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExternalOrderLog> builder)
    {
        builder.ToTable("ExternalOrderLog");

        builder.Property(x => x.TraceId).HasMaxLength(50);
        builder.Property(x => x.Type).HasMaxLength(50).IsRequired();
        builder.Property(x => x.HKNumber).HasMaxLength(100);
        builder.Property(x => x.DocumentaryBarCode).HasMaxLength(100);
        builder.Property(x => x.CustomerOrderNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.OrderRef).HasMaxLength(100);
        builder.Property(x => x.CustomerSerialNumber).HasMaxLength(100);
        builder.Property(x => x.ApiUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ApiMethod).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Ip).HasColumnName("IP").HasMaxLength(200);
        builder.Property(x => x.City).HasMaxLength(50);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.Browser).HasMaxLength(200);
        builder.Property(x => x.Os).HasMaxLength(200);

        builder.HasIndex(x => x.Type).HasDatabaseName("IX_ExternalOrderLog_Type");
        builder.HasIndex(x => x.HKNumber).HasDatabaseName("IX_ExternalOrderLog_HKNumber");
        builder.HasIndex(x => x.DocumentaryBarCode).HasDatabaseName("IX_ExternalOrderLog_DocumentaryBarCode");
        builder.HasIndex(x => x.CustomerOrderNumber).HasDatabaseName("IX_ExternalOrderLog_CustomerOrderNumber");
        builder.HasIndex(x => x.CustomerPONumber).HasDatabaseName("IX_ExternalOrderLog_CustomerPONumber");
        builder.HasIndex(x => x.OrderRef).HasDatabaseName("IX_ExternalOrderLog_OrderRef");
        builder.HasIndex(x => x.CustomerSerialNumber).HasDatabaseName("IX_ExternalOrderLog_CustomerSerialNumber");
    }
}
