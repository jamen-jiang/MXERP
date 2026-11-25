using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库批次项配置
/// </summary>
public sealed class LeaveStockBatchItemConfiguration : IEntityTypeConfiguration<LeaveStockBatchItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockBatchItem> builder)
    {
        builder.ToTable("LeaveStockBatchItem");

        builder.Property(x => x.RelationItemId).IsRequired();
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100);
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.SkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.LeaveStockQty).IsRequired();
        builder.Property(x => x.BillingQty).IsRequired();
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.LeaveStockBatchId).HasDatabaseName("IX_LeaveStockBatchItem_LeaveStockBatchId");
        builder.HasIndex(x => x.PurchaseBatchItemId).HasDatabaseName("IX_LeaveStockBatchItem_PurchaseBatchItemId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_LeaveStockBatchItem_RelationItemId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_LeaveStockBatchItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_LeaveStockBatchItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_LeaveStockBatchItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_LeaveStockBatchItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_LeaveStockBatchItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_LeaveStockBatchItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_LeaveStockBatchItem_WarehouseId");
    }
}
