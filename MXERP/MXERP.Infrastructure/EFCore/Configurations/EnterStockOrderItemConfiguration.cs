using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.EnterStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 入库单项配置
/// </summary>
public class EnterStockOrderItemConfiguration : IEntityTypeConfiguration<EnterStockOrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EnterStockOrderItem> builder)
    {
        builder.ToTable("EnterStockOrderItem");

        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasMany(x => x.EnterStockOrderRelations)
            .WithOne()
            .HasForeignKey(x => x.EnterStockOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EnterStockOrderId).HasDatabaseName("IX_EnterStockOrderItem_EnterStockOrderId");
        builder.HasIndex(x => x.OriginalId).HasDatabaseName("IX_EnterStockOrderItem_OriginalId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_EnterStockOrderItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_EnterStockOrderItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_EnterStockOrderItem_WarehouseId");
    }
}
