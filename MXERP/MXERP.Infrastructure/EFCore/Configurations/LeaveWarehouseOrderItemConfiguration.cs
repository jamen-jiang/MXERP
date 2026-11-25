using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveWarehouseOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出仓单项实体配置
/// </summary>
public class LeaveWarehouseOrderItemConfiguration : IEntityTypeConfiguration<LeaveWarehouseOrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveWarehouseOrderItem> builder)
    {
        builder.ToTable("LeaveWarehouseOrderItem");

        builder.Property(x => x.LeaveStockBatchNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.LeaveWarehouseOrderId).HasDatabaseName("IX_LeaveWarehouseOrderItem_LeaveWarehouseOrderId");
        builder.HasIndex(x => x.LeaveStockBatchId).HasDatabaseName("IX_LeaveWarehouseOrderItem_LeaveStockBatchId");
        builder.HasIndex(x => x.LeaveStockBatchNumber).HasDatabaseName("IX_LeaveWarehouseOrderItem_LeaveStockBatchNumber");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_LeaveWarehouseOrderItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_LeaveWarehouseOrderItem_SalesSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_LeaveWarehouseOrderItem_SpuId");
    }
}
