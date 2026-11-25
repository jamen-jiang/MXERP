using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.BulkOrderItems;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 批量订单项实体配置
/// </summary>
public sealed class BulkOrderItemConfiguration : IEntityTypeConfiguration<BulkOrderItem>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<BulkOrderItem> builder)
    {
        builder.ToTable("BulkOrderItem");
        builder.Property(x => x.SalesSpuCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.PacketInfo).HasMaxLength(500);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasMany(x => x.BulkOrderItemSalesSkus)
            .WithOne()
            .HasForeignKey(x => x.BulkOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.BulkOrderId).HasDatabaseName("IX_BulkOrderItem_BulkOrderId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_BulkOrderItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_BulkOrderItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_BulkOrderItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_BulkOrderItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_BulkOrderItem_SpuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_BulkOrderItem_WarehouseId");
        builder.HasIndex(x => x.IsConfirm).HasDatabaseName("IX_BulkOrderItem_IsConfirm");
    }
}
