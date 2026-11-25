using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库单项配置
/// </summary>
public class LeaveStockOrderItemConfiguration : IEntityTypeConfiguration<LeaveStockOrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockOrderItem> builder)
    {
        builder.ToTable("LeaveStockOrderItem");

        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasMany(x => x.LeaveStockOrderRelations)
            .WithOne()
            .HasForeignKey(x => x.LeaveStockOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.LeaveStockOrderId).HasDatabaseName("IX_LeaveStockOrderItem_LeaveStockOrderId");
        builder.HasIndex(x => x.OriginalId).HasDatabaseName("IX_LeaveStockOrderItem_OriginalId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_LeaveStockOrderItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_LeaveStockOrderItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_LeaveStockOrderItem_WarehouseId");
    }
}
