using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExternalLeaveStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 外部出库单配置
/// </summary>
public sealed class ExternalLeaveStockOrderConfiguration : IEntityTypeConfiguration<ExternalLeaveStockOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExternalLeaveStockOrder> builder)
    {
        builder.ToTable("ExternalLeaveStockOrder");

        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerOrderNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Etc).HasColumnName("ETC");
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.PrintedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.ExternalLeaveStockOrderTrackings)
            .WithOne()
            .HasForeignKey(x => x.ExternalLeaveStockOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_ExternalLeaveStockOrder_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_ExternalLeaveStockOrder_CompanyId");
        builder.HasIndex(x => x.CustomerOrderNumber).HasDatabaseName("IX_ExternalLeaveStockOrder_CustomerOrderNumber");
        builder.HasIndex(x => x.CustomerPONumber).HasDatabaseName("IX_ExternalLeaveStockOrder_CustomerPONumber");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_ExternalLeaveStockOrder_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_ExternalLeaveStockOrder_SalesSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_ExternalLeaveStockOrder_SpuId");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_ExternalLeaveStockOrder_Status");
    }
}
