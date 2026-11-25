using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.BulkOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 批量订单实体配置
/// </summary>
public sealed class BulkOrderConfiguration : IEntityTypeConfiguration<BulkOrder>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<BulkOrder> builder)
    {
        builder.ToTable("BulkOrder");
        builder.Property(x => x.Number).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CustomerOrderNumber).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OrderRef).HasMaxLength(100);
        builder.Property(x => x.CustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.CustomerSerialNumber).HasMaxLength(100);
        builder.Property(x => x.TransportWay).HasMaxLength(50);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PassedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_BulkOrder_ProductType");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_BulkOrder_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_BulkOrder_CompanyId");
        builder.HasIndex(x => x.CustomerOrderNumber).HasDatabaseName("IX_BulkOrder_CustomerOrderNumber");
        builder.HasIndex(x => x.TransportWay).HasDatabaseName("IX_BulkOrder_TransportWay");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_BulkOrder_Status");
    }
}
