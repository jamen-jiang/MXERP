using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.RetailOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 散镜订单项配置
/// </summary>
public sealed class RetailOrderItemConfiguration : IEntityTypeConfiguration<RetailOrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RetailOrderItem> builder)
    {
        builder.ToTable("RetailOrderItem");

        builder.Property(x => x.EyeType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.DegreesType).HasMaxLength(5).IsRequired();
        builder.Property(x => x.Base).HasMaxLength(5);
        builder.Property(x => x.Decenter).HasMaxLength(200);
        builder.Property(x => x.Prism).HasMaxLength(200);
        builder.Property(x => x.Pd).HasColumnName("PD");
        builder.Property(x => x.Ph).HasColumnName("PH");
        builder.Property(x => x.Axis).HasColumnName("AXIS");

        builder.HasIndex(x => x.RetailOrderId).HasDatabaseName("IX_RetailOrderItem_RetailOrderId");
        builder.HasIndex(x => x.EyeType).HasDatabaseName("IX_RetailOrderItem_EyeType");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_RetailOrderItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_RetailOrderItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_RetailOrderItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_RetailOrderItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_RetailOrderItem_SpuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_RetailOrderItem_WarehouseId");
    }
}
